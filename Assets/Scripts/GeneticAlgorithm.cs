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
    // index of gene to mutate
    private static int mutationIndex = 0;
    // cross over point is the index which will indicate where to switch from getting genese from each parent
    private static int crossOverPoint = 0;

    private void Start()
    {
        currentPopulation = GenerateStartingPopulation(populationNumber);
        // pass the starting population of individual chromosomes to be tested
        PerformFitnessTesting(currentPopulation);
        // Sort the population by lowest fitness at the top
        SortPopulation(currentPopulation);
        bestCandidateFound = currentPopulation[0];
        WriteBestCandidateToFile();
        // Keep track of the average fitness for each generation
        averageFitnessPerGeneration = DetermineFitnessOfGeneration();
        WriteAverageFitnessPerGenerationToFile();
        if (currentPopulation.Count < p)
        {
            Debug.LogWarning("Current population less than p!");
            return;
        }
        parent1 = currentPopulation[0];
        parent2 = currentPopulation[1];
        mutationIndex = GetMutationIndex();
        crossOverPoint = GetCrossOverPoint();
        /*#region SortedPop
        foreach (var chromosome in currentPopulation)
        {
            var debugStr = "";
            // Debug.Log("Sorted Fitness: "+ chromosome.FitnessValue);
            foreach (var gene in chromosome)
            {
                debugStr += gene + " | ";
            }
            Debug.Log(debugStr + "=> "+chromosome.FitnessValue);
            debugStr = "";
        }
        //BestCandidatePerGeneration.Add(currentPopulation[0]);
        /*if (BestCandidatePerGeneration.Count > 1)
        {
            SortBestCandidates();
            RemoveAllOtherCandidates();
            var debugStr = "";
            foreach (var gene in BestCandidatePerGeneration[0].Chromosome)
            {
                debugStr += gene + "|";
            }
            Debug.Log("BestCandidateSoFar "+BestCandidatePerGeneration[0].FitnessValue+ " "+debugStr);

        }
        #endregion*/
        /*foreach (var value in averageFitnessPerGeneration)
        {
            Debug.Log(value);
        }
        var strTmp = "";
        /foreach (var individualChromosome in BestCandidatePerGeneration)
        {
            foreach (var gene in individualChromosome.Chromosome)
            {
                strTmp += gene + "|";
            }
            Debug.Log("BestCandidates Chromosome "+strTmp+ " fitnessLevel "+individualChromosome.FitnessValue);
            strTmp = "";
        }*/
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

    /*private void RemoveAllOtherCandidates()
    {
        while (BestCandidatePerGeneration.Count > 1)
        {
            BestCandidatePerGeneration.RemoveAt(BestCandidatePerGeneration.Count - 1);
        }
    }

    private void SortBestCandidates()
    {
        // I followed this tutorial for selection sort for this part of the code
        // https://www.tutorialspoint.com/selection-sort-program-in-chash#:~:text=Selection%20Sort%20is%20a%20sorting,C%23%20is%20given%20as%20follows.
        
        for (int i = 0; i < BestCandidatePerGeneration.Count - 1; i++)
        {
            var smallestFitness = i;
            for (int j = i + 1; j < BestCandidatePerGeneration.Count; j++)
            {
                if (BestCandidatePerGeneration[j].FitnessValue <
                    BestCandidatePerGeneration[smallestFitness].FitnessValue)
                {
                    smallestFitness = j;
                }
            }

            var temp = BestCandidatePerGeneration[smallestFitness];
            BestCandidatePerGeneration[smallestFitness] = BestCandidatePerGeneration[i];
            BestCandidatePerGeneration[i] = temp;
        }
    }*/
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
        Debug.Log(bufferString);
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
    // The exact index which should be mutated in the gene
    private static int GetMutationIndex()
    {
        // avoid mutating the first and last columns
        return rand.Next(1, 7);
    }
    // Mutates a gene in an individual chromosome at the mutation index
    private static IndividualChromosome MutateGene(IndividualChromosome chromosome, int mutationIndex)
    {
        var mutatedChromosome = chromosome;
        var defaultValues = new List<int> {0, 1, 2, 3, 4, 5, 6, 7};
        defaultValues.Remove(chromosome.Chromosome[mutationIndex]);
        var mutatedGeneValue = defaultValues[rand.Next(0, defaultValues.Count)];
        for (int i = 0; i < chromosome.Chromosome.Length; i++)
        {
            if (i == mutationIndex)
            {
                mutatedChromosome.Chromosome[i] = mutatedGeneValue;
            }
        }
        return mutatedChromosome;
    }
    // a queen is going to be represented by which column they are placed in each row
    private static int GenerateGene()
    {
        return rand.Next(0, 8);
    }
    // calculate the fitness level for each individual chromosome
    private static void PerformFitnessTesting(List<IndividualChromosome> population)
    {
        var dbgStr = "";
        foreach (var individualChromosome in population)
        {
            foreach (var gene in individualChromosome)
            {
                dbgStr += gene + " | ";
            }
            CheckFitnessOfIndividualChromosome(individualChromosome);
        }
    }

    private static List<IndividualChromosome> DetermineTop2Fittest(List<IndividualChromosome> populaton)
    {
        // Sort each individual chromosome so that the chromosmes with the fewest collisions is at the top
        // return the top 2 chromsomes from sorted list
        return new List<IndividualChromosome>(2);
    }
    // loop across the chromosomes genes to determine if there are queens attacking other queens in the same row or diagonally
    private static void CheckFitnessOfIndividualChromosome(IndividualChromosome individualChromosome)
    {
        var fitnessChromosomeDBG = "";
        foreach (var gene in individualChromosome.Chromosome)
        {
            fitnessChromosomeDBG += gene + "|";
        }
        Debug.Log(fitnessChromosomeDBG);
        fitnessChromosomeDBG = "";
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