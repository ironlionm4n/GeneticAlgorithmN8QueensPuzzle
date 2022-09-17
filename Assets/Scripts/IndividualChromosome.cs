using System.Collections;

public class IndividualChromosome : IEnumerable
{
    public IndividualChromosome(int[] val)
    {
        _chromosome = val;
        _fitnessValue = 0;
    }
    private int[] _chromosome;
    private int _fitnessValue;

    public int FitnessValue
    {
        get => _fitnessValue;
        set => _fitnessValue = value;
    }
    public int[] Chromosome
    {
        get => _chromosome;
        set => _chromosome = value;
    }

    public IEnumerator GetEnumerator()
    {
        return _chromosome.GetEnumerator();
    }
}
