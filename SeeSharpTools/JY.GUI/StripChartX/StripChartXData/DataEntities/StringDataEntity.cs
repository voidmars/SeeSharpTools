﻿using System;
using System.Collections.Generic;
using SeeSharpTools.JY.GUI.StripChartXUtility;

namespace SeeSharpTools.JY.GUI.StripChartXData.DataEntities
{
    internal class StringDataEntity<TDataType> : DataEntityBase
    {
        private readonly OverLapStrBuffer _xBuffer;
        private readonly List<OverLapWrapBuffer<TDataType>> _yBuffers;

        public StringDataEntity(PlotManager plotManager, DataEntityInfo dataInfo) : base(plotManager, dataInfo)
        {
            _xBuffer = new OverLapStrBuffer(DataInfo.Capacity);
            _yBuffers = new List<OverLapWrapBuffer<TDataType>>(DataInfo.LineCount);
            for (int i = 0; i < DataInfo.LineCount; i++)
            {
                _yBuffers.Add(new OverLapWrapBuffer<TDataType>(DataInfo.Capacity));
            }
        }

        public override int SamplesInChart => _xBuffer.Count;

        public override void AddPlotData(IList<string> xData, Array lineData)
        {
            int dataLength = xData.Count;
            _xBuffer.Add(xData, dataLength);
            int offset = 0;
            for (int i = 0; i < DataInfo.LineCount; i++)
            {
                _yBuffers[i].Add(lineData, dataLength, offset);
                offset += dataLength;
            }
        }

        public override void AddPlotData(DateTime[] startTime, Array lineData)
        {
            throw new NotImplementedException();
        }

        public override void AddPlotData(Array lineData, int sampleCount)
        {
            throw new NotImplementedException();
        }

        public override void GetXYValue(int xIndex, int seriesIndex, ref string xValue, ref string yValue)
        {
            xValue = _xBuffer[xIndex];
            yValue = _yBuffers[seriesIndex][xIndex].ToString();
        }

        public override IList<TDataType> GetPlotDatas<TDataType>(int startIndex, int endIndex)
        {
            throw new NotImplementedException();
        }

        public override void GetMaxAndMinYValue(int seriesIndex, out double maxYValue, out double minYValue)
        {
            ParallelHandler.GetMaxAndMin(_yBuffers[seriesIndex], out maxYValue, out minYValue);
        }

        public override void GetMaxAndMinYValue(out double maxYValue, out double minYValue)
        {
            maxYValue = double.MinValue;
            minYValue = double.MaxValue;
            double tmpMaxYValue = 0;
            double tmpMinYValue = 0;

            foreach (OverLapWrapBuffer<TDataType> yBuffer in _yBuffers)
            {
                ParallelHandler.GetMaxAndMin(yBuffer, out tmpMaxYValue, out tmpMinYValue);
                if (maxYValue < tmpMaxYValue)
                {
                    maxYValue = tmpMaxYValue;
                }
                if (minYValue > tmpMinYValue)
                {
                    minYValue = tmpMinYValue;
                }
            }
        }

        public override void Clear()
        {
            base.Clear();
            _xBuffer.Clear();
            foreach (OverLapWrapBuffer<TDataType> yBuffer in _yBuffers)
            {
                yBuffer.Clear();
            }
        }
    }
}