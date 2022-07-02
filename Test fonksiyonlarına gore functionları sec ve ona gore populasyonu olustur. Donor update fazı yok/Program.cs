using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;

namespace ffp
{
    class Program
    {

        static void Main(string[] args)
        {
			string[] operators = { "+", "-", "*", "/" };
			// string[] functions = { "sin", "cos", "exp", "rlog" };
			string[] functions = { "same" };
			string[] terminals_1V = { "x", "1" };
			string[] terminals_2V = { "x", "y" };

		//	string[] param_functions = { "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8" };
			string[] param_functions = { "f3" };
			int[] param_nod = { 125 };
			int[] param_nor = { 125 };
		//	int[] param_evals = { 25000, 50000, 100000 };
			int[] param_evals = { 25000 };
			int[] param_populations = { 250 };
			int param_maxRun = 10;

			Par.NoD					= 3;
			Par.NoR					= 3;
			Par.pop_size          	= 50;
			Par.max_eval          	= 25000; // 25000
			Par.max_gen           	= 1000;
			Par.init_min_depth    	= 0;
			Par.init_max_depth    	= 6;
			Par.max_depth         	= 15;
			Par.max_depth_value		= "1000000";
			Par.alpha				= 0.1;
			Par.beta				= 0.5;
			Par.gamma				= 1.0;
			Par.param_currentRun	= 1;
			Tree.operator_rate 		= 0.5;
			Tree.terminal_rate 		= 0.5;
			Tree.sharing_rate		= 0.9;


			
			string filename;

			foreach (string func in param_functions)
			{
				Par set_parameters = new Par(func);

				if (Par.var_num == 1)
					Tree.terminals = (string[])terminals_1V.Clone();
				else
					Tree.terminals = (string[])terminals_2V.Clone();

				if (func == "f1" || func == "f2")
				{
					Tree.functions = new string[] { "same" };
				}

				if (func == "f3" || func == "f4" || func == "f7" || func == "f8")
				{
					Tree.functions = new string[] { "sin", "cos" };
				}

				if (func == "f5")
				{
					Tree.functions = new string[] { "rlog" };
				}

				if (func == "f6")
				{
					Tree.functions = new string[] { "sin", "cos", "exp", "rlog" };
				}


				foreach (int eval in param_evals)
				{
					Par.max_eval = eval;

					foreach (int population in param_populations)
					{
						Par.pop_size = population;

						foreach (int nod in param_nod)
						{
							Par.NoD = nod;

							foreach (int nor in param_nor)
							{
								Par.NoR = nor;

								filename = $"{Par.max_eval}_{Par.pop_size}_{Par.SELECTED}_{Par.NoD}_{Par.NoR}.txt";

								for (int run = 1; run <= param_maxRun; run++)
								{
									Par.param_currentRun = run;
									IPA model = new IPA();
									model.Run(filename);
								}
							}
						}
					}
				}
			}
		}
	}
}
