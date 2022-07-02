using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffp
{
    internal class IPA
    {
        public static int counter = 0;
        public Tree[] population = null;
        public Tree[] donors = null;
        public Tree[] receivers = null;
        public double[] errors = new double[Par.pop_size];
        public Tree best;
        public static Boolean print_result = true;
        public int[] doseControl = new int[Par.NoR];
        public int[] treatmentControl = new int[Par.NoR];
        public double[] bestErrors = new double[Par.max_eval];

        public IPA()
        {
            counter = 0;
            population = null;
            donors = null;
            receivers = null;
            errors = new double[Par.pop_size];
            best = null;
            print_result = true;
            doseControl = new int[Par.NoR];
            treatmentControl = new int[Par.NoR];
            bestErrors = new double[Par.max_eval];

            this.setTempArrays();

            // Initialize Population
            this.population = Functions.Init_population();

            int min_index = 0;
            double best_error = 10e6;
            for (int i = 0; i < Par.pop_size; i++)
            {
                this.errors[i] = this.population[i].error;
                if (this.population[i].error <= best_error)
                {
                    best_error = this.population[i].error;
                    min_index = i;
                }
            }
            this.best = (Tree)this.population[min_index].Clone();
            this.best.Update_error();
        }

        public void StoreBestMemberOfEachEvaluation()
        {
            if (counter <= Par.max_eval)
                this.bestErrors[counter - 1] = this.best.error;
        }

        public void setTempArrays()
        {
            for (int i = 0; i < Par.NoR; i++)
            {
                doseControl[i] = 1;
                treatmentControl[i] = 1;
            }
        }

        public void PrintMembersOfPopulation()
        {
            for (int i = 0; i < Par.pop_size; i++)
            {
                Console.WriteLine(i + " => " + this.population[i].error + " => " + this.population[i].Tree_equation());
            }
        }

        public void PrintDonors()
        {
            for (int i = 0; i < Par.NoD; i++)
            {
                Console.WriteLine("Donor " + i + " => " + this.donors[i].error + " => " + this.donors[i].Tree_equation());
            }
        }

        public void PrintReceivers()
        {
            for (int i = 0; i < Par.NoR; i++)
            {
                Console.WriteLine("Receiver " + i + " => " + this.receivers[i].error + " => " + this.receivers[i].Tree_equation());
            }
        }

        public void PrintErrors()
        {
            for (int i = 0; i < Par.max_eval; i++)
            {
                Console.WriteLine(this.bestErrors[i]);
            }
        }

        public void Evaluate(int current, Tree temp)
        {
            if (temp.error < this.population[current].error)
            {
                this.population[current] = (Tree)temp.Clone();
                this.population[current].Update_error();
                this.errors[current] = this.population[current].error;

                if (this.population[current].error < this.best.error)
                {
                    this.best = (Tree)this.population[current].Clone();
                    if (print_result)
                        Console.WriteLine("Counter: " + counter + "\t| Best: " + this.best.error);
                }
            }

            this.StoreBestMemberOfEachEvaluation();
        }

        public void EvaluateTransfer(Tree receiver, Tree donor, int receiverIndex)
        {
            if (this.doseControl[receiverIndex] == 1)
            {
                if (receiver.error < donor.error)
                {
                    doseControl[receiverIndex] = doseControl[receiverIndex] + 1;
                    this.population[Par.pop_size - (receiverIndex + 1)] = (Tree)receiver.Clone();
                    this.receivers[receiverIndex] = (Tree)receiver.Clone();
                    this.errors[Par.pop_size - (receiverIndex + 1)] = this.population[Par.pop_size - (receiverIndex + 1)].error;
                }
                else
                {
                    this.population[Par.pop_size - (receiverIndex + 1)] = (Tree)donor.Clone();
                    this.errors[Par.pop_size - (receiverIndex + 1)] = this.population[Par.pop_size - (receiverIndex + 1)].error;
                    treatmentControl[receiverIndex] = 0;
                }
            } 
            else
            {
                if (receiver.error < this.population[receiverIndex].error)
                {
                    this.population[Par.pop_size - (receiverIndex + 1)] = (Tree)receiver.Clone();
                    this.receivers[receiverIndex] = (Tree)receiver.Clone();
                    this.errors[Par.pop_size - (receiverIndex + 1)] = this.population[Par.pop_size - (receiverIndex + 1)].error;
                }
                else
                {
                    treatmentControl[receiverIndex] = 0;
                }
            }

            // check new member is better than best member ?
            if (this.population[Par.pop_size - (receiverIndex + 1)].error < this.best.error)
            {
                this.best = (Tree)this.population[Par.pop_size - (receiverIndex + 1)].Clone();
                if (print_result)
                    Console.WriteLine("Counter: " + counter + "\t| Best: " + this.best.error);
            }

            this.StoreBestMemberOfEachEvaluation();
        }

        public void EvaluateDonor(Tree donor)
        {
            if (donor.error < this.best.error)
            {
                this.best = (Tree)donor.Clone();
                if (print_result)
                    Console.WriteLine("Counter: " + counter + "\t| Best: " + this.best.error);
            }

            this.StoreBestMemberOfEachEvaluation();
        }

        public Tree[] SelectDonors()
        {
            Tree[] pop = new Tree[Par.NoD];

            for (int i = 0; i < Par.NoD; i++)
            {
                pop[i] = (Tree)this.population[i].Clone();
            }
           
            return pop;
        }

        public Tree[] SelectReceivers()
        {
            Tree[] pop = new Tree[Par.NoR];

            for (int i = 0; i < Par.NoR; i++)
            {
                pop[i] = (Tree)this.population[Par.pop_size - (i + 1)].Clone();
            }

            return pop;
        }

        public void IPA_algorithm()
        {
            while (counter < Par.max_eval)
            {
                // Enfeksiyon Yayılım Fazı
                Array.Sort(this.errors, this.population);

                for (int i = 0; i < Par.pop_size; i++)
                {
                    if (counter > Par.max_eval)
                        break;

                    counter += 1;

                    int rand;
                    do
                    {
                        Random rnd = new Random();
                        rand = rnd.Next(0, Par.pop_size);
                    } while (rand == i);

                    Tree temp = (Tree)this.population[rand].Clone();
                    Tree sub = (Tree)temp.Copy_subtree();

                    Tree currentClone = (Tree)this.population[i].Clone();
                    currentClone.Paste_subtree(sub);
                    currentClone.Change_subtree();
                    currentClone.Update_error();
                    currentClone = Functions.Check_depth(currentClone);

                    this.Evaluate(i, currentClone);
                }

                // Plazma Transferi Fazı
                Array.Sort(this.errors, this.population);

                this.donors = this.SelectDonors();
                this.receivers = this.SelectReceivers();

                for (int i = 0; i < Par.NoR; i++)
                {
                    while (treatmentControl[i] == 1)
                    {
                        if (counter > Par.max_eval)
                            break;

                        counter += 1;

                        Random rnd = new Random();
                        int randDonor = rnd.Next(0, Par.NoD);

                        Tree tempDonor = (Tree)this.donors[randDonor].Clone();
                        Tree subDonor = (Tree)tempDonor.Copy_subtree();

                        Tree tempReceiver = (Tree)this.receivers[i].Clone();
                        tempReceiver.Paste_subtree(subDonor);
                        tempReceiver.Update_error();
                        tempReceiver = Functions.Check_depth(tempReceiver);

                        this.EvaluateTransfer(tempReceiver, tempDonor, i);
                    }
                }

                // Donör Update Fazı
                for (int i = 0; i < Par.NoD; i++)
                {
                    if (counter > Par.max_eval)
                        break;

                    counter += 1;

                    Random rnd = new Random();
                    double randomDouble = rnd.NextDouble();

                    if ((double)counter / Par.max_eval < randomDouble)
                    {
                        this.donors[i].Change_subtree();
                        this.donors[i].Update_error();
                        this.population[i] = (Tree)this.donors[i].Clone();
                    }
                    else
                    {
                        if (Par.SELECTED == "f1" || Par.SELECTED == "f2")
                        {
                            Tree.functions = new string[] { "same" };
                        }

                        if (Par.SELECTED == "f3" || Par.SELECTED == "f4" || Par.SELECTED == "f7" || Par.SELECTED == "f8")
                        {
                            Tree.functions = new string[] { "sin", "cos" };
                        }

                        if (Par.SELECTED == "f5")
                        {
                            Tree.functions = new string[] { "rlog" };
                        }

                        if (Par.SELECTED == "f6")
                        {
                            Tree.functions = new string[] { "sin", "cos", "exp", "rlog" };
                        }

                        Tree newDonor = new Tree();
                        newDonor.Create_tree("full", Par.init_min_depth, Par.init_max_depth);
                        newDonor.Update_error();
                        this.donors[i] = (Tree)newDonor.Clone();
                        this.population[i] = (Tree)newDonor.Clone();
                    }
                    this.EvaluateDonor(this.donors[i]);
                }
            }
        }

        public async void Run(string filename = "")
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            print_result = false;
           // this.PrintMembersOfPopulation();
            this.IPA_algorithm();
            // this.PrintErrors();
           // Console.WriteLine("best error : " + this.best.error);
           // Console.WriteLine("best model : " + this.best.Tree_equation());
            Console.WriteLine(filename + " => " + Par.param_currentRun + " => " + this.best.error + " => " + this.best.Tree_equation());
            watch.Stop();
    
            
            
            /*
            var elapsedMs = watch.ElapsedMilliseconds;

            var joinedErrors = String.Join("_", this.bestErrors);

            string experimentalSummary = $"{Par.param_currentRun.ToString()}\t{this.best.Tree_equation()}\t{this.best.error}\t{elapsedMs}\t{joinedErrors}";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@$"C:\Users\hekim\OneDrive\Masaüstü\IPA-Programming\new_results\{filename}", true))
                file.WriteLine(experimentalSummary);
            */
        }
    }
}
