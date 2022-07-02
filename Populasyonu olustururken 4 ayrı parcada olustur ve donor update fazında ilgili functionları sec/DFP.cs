using System;
using System.Collections.Generic;
using System.Text;

namespace ffp
{
    class DFP
    {
        // class static variables
        public static int counter = 0;
        public static int gen = 0;
        public static Boolean print_result = false;
        public static double alpha = Par.alpha;
        public static double beta = Par.beta;
        public static double gamma = Par.gamma;

        // instance attributes
        public Tree[] population = null;
        public Tree best;
        public int best_gen;
        public double[] errors = new double[Par.pop_size];


        public DFP()
        {
            this.population = Functions.Init_population();
            int min_index = -1;
            double best_error = 10e6;
            for (int i = 0; i < Par.pop_size; i++)
            {
                this.errors[i] = this.population[i].error;
                if (this.population[i].error <= best_error)
                {
                    min_index = i;
                }
            }
            this.best = (Tree)this.population[min_index].Clone();
            this.best.Update_error();
        }

        public Tree attract(int i, int j)
        {
            double distance = Math.Abs(this.population[i].error - this.population[j].error);
            Tree temp = (Tree)this.population[i].Clone();

            if (distance > gamma)
            {
                for (int k = 0; k < 2; k++)
                    Functions.Share(this.population[j], temp);
                temp.Change_node();
            }
            else if (distance > beta && distance <= gamma)
            {
                Functions.Share(this.population[j], temp);
                temp.Change_node();
            }
            else if (distance > alpha && distance <= beta)
            {
                Functions.Share(this.population[j], temp);
            }
            else
                temp.Change_node();

            return temp;
        }

        public void Evaluate(int current, Tree temp)
        {

            if (temp.error < this.population[current].error)
            {
                temp.Fix_tree();
                this.population[current] = (Tree)temp.Clone();
                this.errors[current] = this.population[current].error;
                if (this.population[current].error < this.best.error)
                {
                    this.best = (Tree)this.population[current].Clone();
                    this.best_gen = gen;
                    if (print_result)
                        Console.WriteLine("Counter: " + counter + "\t| Gen: " + gen + "\t| Best: " + this.best.error);
                }
            }
        }

        public void Firefly_algorithm()
        {
            while (counter < Par.max_eval)
            {
                Array.Sort(this.errors, this.population);

                for (int i = 0; i < Par.pop_size; i++)
                {
                    if (counter > Par.max_eval)
                        break;

                    for (int j = 0; j < Par.pop_size; j++)
                    {
                        if (counter > Par.max_eval)
                            break;


                        if (this.population[i].error >= this.population[j].error)
                        {
                            counter += 1;

                            Tree temp = (Tree) this.attract(i, j);
                            temp = Functions.Check_depth(temp);
                            // temp.Simplify_tree();
                            Boolean different = Functions.Control_difference(this.population, temp);
                            while (!different)
                            {
                                string method;
                                Random rnd = new Random();
                                int rand = rnd.Next(0, 2);
                                if (rand == 0)
                                    method = "grow";
                                else
                                    method = "full";
                                temp.Create_tree(method, Par.init_min_depth, Par.init_max_depth);
                                different = Functions.Control_difference(this.population, temp);
                            }
                            temp.Update_error();
                            this.Evaluate(i, temp);
                        }

                        if (this.best.error < 0.01)
                            break;
                    }
                    if (this.best.error < 0.01)
                        break;
                }
                if (this.best.error < 0.01)
                    break;
                gen += 1;
            }
        } // end of Firefly_algorithm method

        public void Run()
        {
            print_result = true;
            this.Firefly_algorithm();
            Console.WriteLine(this.best.error);
            Console.WriteLine(this.best.Tree_equation());
        }
    }
}