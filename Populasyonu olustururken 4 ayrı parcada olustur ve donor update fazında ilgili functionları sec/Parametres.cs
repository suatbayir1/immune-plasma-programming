using System;
using System.Collections.Generic;
using System.Text;

namespace ffp
{
    class Par
    {   
        public static string SELECTED       = "";
		public static int pop_size          = 4;
		public static int group_num         = 5;
		public static int max_eval          = 25000; 
        public static int max_gen           = 1000;
		public static int init_min_depth    = 0;
		public static int init_max_depth    = 6;
		public static int max_depth         = 15;
		public static string max_depth_value = "1000000";
        public static double[] domain_x     = {-1, 1};
        public static double[] domain_y     = {-1, 1};
		public static int point_num         = 20;
		public static int var_num           = 1;
        public static double alpha          = 0.5;
        public static double beta           = 1.0;
        public static double gamma          = 1.5;
        public static Boolean Simplify      = false;
        public static int NoD               = 1;
        public static int NoR               = 1;
        public static int param_currentRun  = 1;

        public Par(string func)
        {
            if (func == "f1")
                set_to_f1();
            else if (func == "f2")
                set_to_f2();
            else if (func == "f3")
                set_to_f3();
            else if (func == "f4")
                set_to_f4();
            else if (func == "f5")
                set_to_f5();
            else if (func == "f6")
                set_to_f6();
            else if (func == "f7")
                set_to_f7();
            else if (func == "f8")
                set_to_f8();
            else
            {
                Console.WriteLine("Select a valid function!!");
                Environment.Exit(0);
            }
            Par.max_depth_value   = "1";
        }


        public static void set_to_f1()
        {
            SELECTED    = "f1";
            domain_x[0] = -1;
            domain_x[1] = 1;
            point_num   = 20;
            var_num     = 1;
        }

        public static void set_to_f2()
        {
            SELECTED    = "f2";
            domain_x[0] = -1;
            domain_x[1] = 1;
            point_num   = 20;
            var_num     = 1; 
        }

        public static void set_to_f3()
        {
            SELECTED    = "f3";
            domain_x[0] = 0;
            domain_x[1] = 1;
            point_num   = 20;
            var_num     = 1; 
        }

        public static void set_to_f4()
        {
            SELECTED    = "f4";
            domain_x[0] = 0;
            domain_x[1] = Math.PI / 2;
            point_num   = 20;
            var_num     = 1; 
        }

        public static void set_to_f5()
        {
            SELECTED    = "f5";
            domain_x[0] = 0;
            domain_x[1] = 2;
            point_num   = 20;
            var_num     = 1; 
        }

        public static void set_to_f6()
        {
            SELECTED    = "f6";
            domain_x[0] = 0;
            domain_x[1] = 4;
            point_num   = 20;
            var_num     = 1;
        }

        public static void set_to_f7()
        {
            SELECTED    = "f7";
            domain_x[0] = 0;
            domain_x[1] = 1.1;
            domain_y[0] = 0;
            domain_y[1] = 1;
            point_num   = 100;
            var_num     = 2;
        }

        public static void set_to_f8()
        {
            SELECTED    = "f8";
            domain_x[0] = 0;
            domain_x[1] = 1.1;
            domain_y[0] = 0;
            domain_y[1] = 1;
            point_num   = 100;
            var_num     = 2;
        }

        public static double Target_function(double x, double y)
		{
            if (SELECTED == "f1")
                return Math.Pow(x, 4) + Math.Pow(x, 3) + Math.Pow(x, 2) + x;
            else if (SELECTED == "f2")
                return Math.Pow(x, 5) + Math.Pow(x, 4) + Math.Pow(x, 3) + Math.Pow(x, 2) + x;
            else if (SELECTED == "f3")
                return Math.Sin(x) + Math.Sin(x + Math.Pow(x, 2));
            else if (SELECTED == "f4")
                return Math.Sin(Math.Pow(x, 2)) * Math.Cos(x) - 1;
            else if (SELECTED == "f5")
                return Math.Log(x + 1) + Math.Log(Math.Pow(x, 2) + 1);
            else if (SELECTED == "f6")
                return Math.Sqrt(x);
            else if (SELECTED == "f7")
                return Math.Sin(x) + Math.Sin(Math.Pow(y, 2));
            else if (SELECTED == "f8")
                return 2 * Math.Sin(x) * Math.Cos(y);
            else
                return 0;
        }
    }
}