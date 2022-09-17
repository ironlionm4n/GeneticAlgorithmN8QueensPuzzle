using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

/*
 * Solve the n-Queens where n=8 problem using a genetic algorithm. Your initial population size should be 8.
 * The fitness function will check the total number of attacks - sum up, for each piece, the number of pieces being attacked
 * (some of these may be counted more than once and that is fine).
 * We want to find / favor solutions that minimize the number of attacks.
 * Track the average fitness for each generation and graph those result for 1000 generations.
 * Track the best candidate seen thus far and graph that across the generations as well.
 * Submit the code, a video of it running, and your graphs.
 */

public class GeneticAlgorithm : MonoBehaviour
{
    private static Random rand = new();
    // the chance that a gene will mutate
    private static float mutationRate = 25.0f;
    // Size of board - number of rows and columns
    private static int n = 8;
    // initial population size
    private static int populationNumber = n;
    // starting population list of individual chromosome objects
    public List<IndividualChromosome> currentPopulation = new();
    // list of instantiated white and black squares in the scene
    public static List<GameObject> squares = new();
    // list of average fitness calculated per generation
    private static float averageFitnessPerGeneration;
    // list of most fitted board states
    //private static List<IndividualChromosome> BestCandidatePerGeneration = new();
    // value of fitness for the best candidate found so far
    private static IndividualChromosome bestCandidateFound;
    // parent1 for creating new generation
    private static IndividualChromosome parent1;
    // parent2 for creating new generation
    private static IndividualChromosome parent2;
    // number of parents to produce offspring
    private static int p = 2;
    // cross over point is the index which will indicate where to switch from getting genese from each parent
    private static int crossOverPoint = 0;

    private void Start()
    {
        var generations = 0;
        do
        {
            if (generations == 0)
                currentPopulation = GenerateStartingPopulation(populationNumber);
            PerformFitnessTesting(currentPopulation);
            averageFitnessPerGeneration = DetermineFitnessOfGeneration();
            WriteAverageFitnessPerGenerationToFile();
            SortPopulation(currentPopulation);
            bestCandidateFound = currentPopulation[0];
            WriteBestCandidateToFile();
            parent1 = currentPopulation[0];
            parent2 = currentPopulation[1];
            currentPopulation.Clear();
            CrossOverParents(parent1, parent2);
            Debug.Log("currentPopulation after crossover size: "+currentPopulation.Count);
            foreach (var individualChromosome in currentPopulation)
            {
                PerformMutations(individualChromosome);
            }
            generations++;
        } while (generations < 10000);

    }
    private void CrossOverParents(IndividualChromosome parent1, IndividualChromosome parent2)
    {
        while (currentPopulation.Count <= 6)
        {
            crossOverPoint = GetCrossOverPoint();
            int[] child1Genes = new int[n];
            int[] child2Genes = new int[n];
            for (int i = 0; i < n; i++)
            {
                if (i < crossOverPoint)
                {
                    child1Genes[i] = parent1.Chromosome[i];
                    child2Genes[i] = parent2.Chromosome[i];
                } else if (i >= crossOverPoint)
                {
                    child1Genes[i] = parent2.Chromosome[i];
                    child2Genes[i] = parent1.Chromosome[i];
                }
            }

            IndividualChromosome child1 = new IndividualChromosome(child1Genes);
            IndividualChromosome child2 = new IndividualChromosome(child2Genes);
            currentPopulation.Add(child1);
            currentPopulation.Add(child2);
        }
    }

    // cross over each parents genes starting with parent1 until the cross over point then back fill with parent2 genes
    // switch order of parent1 and parent2 for second offspring
    private int GetCrossOverPoint()
    {
        // avoid crossing over at the edges of the chromosome - leads to better variation
        return rand.Next(2, 6);
    }

    // File to keep track of the average fitness per generation
    private void WriteAverageFitnessPerGenerationToFile()
    {
        string path =
            @"C:\Users\ironl\Desktop\School Stuff\Fall2022\AIGames\NQueensGeneticAlgorithm\AverageFitnessPerGeneration.txt";
        if (!File.Exists(path))
        {
            Debug.Log("File Not Found");
            return;
        }
        using var sw = File.AppendText(path);
        sw.WriteLine(averageFitnessPerGeneration);
        sw.Close();
    }
    // write the board state and the fitness value of the best candidate per generation
    private static void WriteBestCandidateToFile()
    {
        string path =
            @"C:\Users\ironl\Desktop\School Stuff\Fall2022\AIGames\NQueensGeneticAlgorithm\BestCandidatesFound.txt";
        if (!File.Exists(path)) return;
        var buffer = new string[9];
        var counter = 0;
        foreach (var gene in bestCandidateFound.Chromosome)
        {
            buffer[counter] = gene.ToString()+"|";
            counter++;
        }
        buffer[counter] = " FV: "+bestCandidateFound.FitnessValue.ToString() + ",";
        var bufferString = "";
        foreach (var str in buffer)
        {
            bufferString += str;
        }
        //Debug.Log(bufferString);
        using var sw = File.AppendText(path);
        sw.WriteLine(bufferString);
        sw.Close();
    }
    
    // add up the total fitenss for each chromosome in this generation, then divide by the population size to get the average fitness
    // this value should go down across generations
    private float DetermineFitnessOfGeneration()
    {
        float totalFitness = 0f;
        foreach (var chromosome in currentPopulation)
        {
            totalFitness += chromosome.FitnessValue;
        }
        return totalFitness / currentPopulation.Count;
    }

    // Sort population so that the chromosomes with the smallest number of collisions is at the top of list
    private void SortPopulation(List<IndividualChromosome> individualChromosomes)
    {
        // I followed this tutorial for selection sort for this part of the code
        // https://www.tutorialspoint.com/selection-sort-program-in-chash#:~:text=Selection%20Sort%20is%20a%20sorting,C%23%20is%20given%20as%20follows.

        for (int i = 0; i < currentPopulation.Count - 1; i++)
        {
            var smallestFitness = i;
            for (int j = i + 1; j < currentPopulation.Count; j++)
            {
                if (individualChromosomes[j].FitnessValue < individualChromosomes[smallestFitness].FitnessValue)
                {
                    smallestFitness = j;
                }
            }
            var temp = individualChromosomes[smallestFitness];
            individualChromosomes[smallestFitness] = individualChromosomes[i];
            individualChromosomes[i] = temp;
        }
    }

    // Create board states equal to size of the starting population
    private static List<IndividualChromosome> GenerateStartingPopulation(int startingPopulation)
    {
        List<IndividualChromosome> returnList = new List<IndividualChromosome>();
        for (int i = 0; i < startingPopulation; i++)
        {
            returnList.Add(new IndividualChromosome(GenerateChromosome()));
        }
        return returnList;
    }
    // each chromosome represents a board state and will have 8 genes which represent the place of each queen
    private static int[] GenerateChromosome()
    {
        int[] chromosome = new int[n];
        for (int i = 0; i < 8; i++)
        {
            chromosome[i] = GenerateGene();
        }
        return chromosome;
    }
    // Mutates a gene in an individual chromosome at the mutation index
    private static void PerformMutations(IndividualChromosome chromosome)
    {
        for (int i = 0; i < chromosome.Chromosome.Length; i++)
        {
            var shouldMutate = rand.Next(0, 101) <= mutationRate;
            Debug.Log("mutationRate: "+mutationRate+" shouldMutate: "+shouldMutate);
            if (shouldMutate)
            {
                int mutatedGeneValue;
                do
                {
                    mutatedGeneValue = rand.Next(0, 8);
                } while (mutatedGeneValue == chromosome.Chromosome[i]);
                Debug.Log("mutatedGeneValue: "+mutatedGeneValue+" chromosome[i] value: "+chromosome.Chromosome[i]);
                chromosome.Chromosome[i] = mutatedGeneValue;
            }
        }
    }
    // a queen is going to be represented by which column they are placed in each row
    private static int GenerateGene()
    {
        return rand.Next(0, 8);
    }
    // calculate the fitness level for each individual chromosome
    private static void PerformFitnessTesting(List<IndividualChromosome> population)
    {
        foreach (var individualChromosome in population)
        {
            CheckFitnessOfIndividualChromosome(individualChromosome);
        }
    }

    // loop across the chromosomes genes to determine if there are queens attacking other queens in the same row or diagonally
    private static void CheckFitnessOfIndividualChromosome(IndividualChromosome individualChromosome)
    {
        /*var fitnessChromosomeDBG = "";
        foreach (var gene in individualChromosome.Chromosome)
        {
            fitnessChromosomeDBG += gene + "|";
        }
        Debug.Log(fitnessChromosomeDBG);*/
        //fitnessChromosomeDBG = "";
        for (int i = 0; i < 8; i++)
        {
            for (int j = i; j < 7; j++)
            {
                if (individualChromosome.Chromosome[i] == individualChromosome.Chromosome[j + 1])
                {
                    //Debug.Log("Collision i = " + i + " " + individualChromosome.Chromosome[i] + " = j = "+j+" == " +individualChromosome.Chromosome[j+1]);
                    individualChromosome.FitnessValue++;
                }
                if (Mathf.Abs(individualChromosome.Chromosome[i] - individualChromosome.Chromosome[j + 1]) == 1)
                {
                    //Debug.Log("Collision (i = " + i + ") "+ individualChromosome.Chromosome[i] + " - (j = "+j+") "+individualChromosome.Chromosome[j+1]+ "| = 1");
                    individualChromosome.FitnessValue++;
                }
            }
        }
        //Debug.Log("Fitness Level: "+individualChromosome.FitnessValue);
    }
}