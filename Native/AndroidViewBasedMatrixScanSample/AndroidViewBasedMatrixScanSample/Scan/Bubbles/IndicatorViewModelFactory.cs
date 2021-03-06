﻿using System;
using System.Collections.Generic;
using Android.Content;
using Java.Lang;
using Java.Text;
using Java.Util;
using ScanditBarcodePicker.Android.Recognition;


namespace AndroidViewBasedMatrixScanSample.Scan.Bubbles
{
    public sealed class IndicatorViewModelFactory
    {
        readonly SimpleDateFormat printDateFormat = new SimpleDateFormat("MM-dd-yy");
        readonly Context context;

        public IndicatorViewModelFactory(Context context)
        {
            this.context = context;
        }

        public BaseBubble CreateBubble(IndicatorState indicatorState, TrackedBarcode barcode)
        {
            BubbleData barcodeData = MockBubbleDataObject(barcode.Data);

            switch (indicatorState)
            {
                case IndicatorState.MINIMISED:
                    return new MinimisedBubble(context, barcodeData);
                case IndicatorState.MAXIMISED:
                    return new MaximisedBubble(context, barcodeData);
                default:
                    return new NoBubble(context, barcodeData);
            }
        }

        private BubbleData MockBubbleDataObject(string data)
        {
            IList<string> list = new List<string>();
            list.Add(data);
            list.Add(MockBubbleDataObjectStock(data));
            list.Add(MockBubbleDataObjectOnline());
            list.Add(MockBubbleDataObjectDeliveryDate(data));

            return new BubbleData(list);
        }

        private string MockBubbleDataObjectStock(string data)
        {
            int lastDigit = Java.Lang.Math.Max(Character.GetNumericValue(data[data.Length - 1]), 0);
            int stockValue;
            if (lastDigit <= 2)
            {
                stockValue = lastDigit % 5; // Critical: 0-5
            }
            else if (lastDigit <= 5)
            {
                int codeValue;
                try
                {
                    codeValue = Int32.Parse(data.Substring(data.Length - 2));
                }
                catch (NumberFormatException)
                {
                    codeValue = 0;
                }
                catch (ArgumentNullException)
                {
                    codeValue = 0;
                }
                catch (OverflowException)
                {
                    codeValue = 0;
                }


                stockValue = 11 + (codeValue); // Good: 11+
            }
            else
            {
                stockValue = 6 + lastDigit % 5; // Low: 6-10
            }

            return stockValue.ToString();
        }

        string MockBubbleDataObjectOnline()
        {
            return "15";
        }

        string MockBubbleDataObjectDeliveryDate(string data)
        {
            Date date = new Date();
            long codeValue;
            try
            {
                codeValue = Long.ParseLong(data);
            }
            catch (NumberFormatException)
            {
                codeValue = 0;
            }
            catch (ArgumentNullException)
            {
                codeValue = 0;
            }
            catch (OverflowException)
            {
                codeValue = 0;
            }
            return printDateFormat.Format(AddDays(date, (int)(codeValue % 15)));
        }

        Date AddDays(Date date, int days)
        {
            GregorianCalendar cal = new GregorianCalendar();
            cal.Time = date;
            cal.Add(CalendarField.Date, days);
            return cal.Time;
        }
    }
}
