/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Models.Prediction
{
	public class RankingEvaluationResult
	{
		public RankingEvaluationResult(Dictionary<string,double> defectCodeSizeByFile, string[] predictedDefectFiles)
		{
			DefectCodeSize = defectCodeSizeByFile.Sum(x => x.Value);
			DefectCodeSizeInSelection = defectCodeSizeByFile
				.Where(x => predictedDefectFiles.Any(y => y == x.Key))
				.Sum(x => x.Value);
		}
		public double DefectCodeSize
		{
			get; private set;
		}
		public double DefectCodeSizeInSelection
		{
			get; private set;
		}
		public double DefectCodeSizeInSelectionPercent
		{
			get { return (DefectCodeSizeInSelection / DefectCodeSize) * 100; }
		}
	}
}
