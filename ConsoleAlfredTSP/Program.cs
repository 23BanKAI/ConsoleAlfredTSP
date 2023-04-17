class TSPHopfieldNetwork
{
    static int N = 6; // Number of cities
    static double[,] W = new double[N, N]; // Weight matrix
    static int[] path = new int[N]; // Best path found by the algorithm
    static double minLength = Double.PositiveInfinity; // Length of the best path found

    static void Main()
    {
        // Initialize the weight matrix
        double[,] D = { {0, 5, 2, 4, 9, 1},
                        {5, 0, 3, 9, 3, 7},
                        {2, 3, 0, 5, 6, 8},
                        {4, 9, 5, 0, 2, 7},
                        {9, 3, 6, 2, 0, 1},
                        {1, 7, 8, 7, 1, 0}};
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                W[i, j] = -D[i, j];
            }
        }

        // Run the Hopfield Network algorithm
        int[] perm = new int[N];
        for (int i = 0; i < N; i++)
        {
            perm[i] = i;
        }
        int[] input = new int[N * N];
        int[] output = new int[N * N];
        HopfieldNetwork hopfield = new HopfieldNetwork(N * N);
        int iterations = 1000;
        Random rand = new Random();
        for (int i = 0; i < iterations; i++)
        {
            input = Shuffle(perm, rand);
            hopfield.SetInput(input);
            hopfield.Update();
            output = hopfield.GetOutput();
            int[] path = Decode(output);
            double length = GetLength(path, D);
            if (length < minLength)
            {
                minLength = length;
                Array.Copy(path, TSPHopfieldNetwork.path, N);
            }
        }

        // Print the best path found
        Console.Write("Best Path: ");
        for (int i = 0; i < N; i++)
        {
            Console.Write(TSPHopfieldNetwork.path[i] + 1 + " ");
        }
        Console.WriteLine();
        Console.WriteLine("Length: " + TSPHopfieldNetwork.minLength);
    }

    static int[] Shuffle(int[] perm, Random rand)
    {
        int[] shuffled = new int[N * N];
        Array.Copy(perm, shuffled, N);
        for (int i = 0; i < N - 1; i++)
        {
            int j = rand.Next(i, N);
            int temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }
        return shuffled;
    }

    static int[] Decode(int[] output)
    {
        int[] path = new int[N];
        bool[] used = new bool[N];
        int index = 0;
        for (int i = 0; i < N * N; i++)
        {
            int j = output[i];
            if (!used[j])
            {
                path[index++] = j;
                used[j] = true;
            }
        }
        return path;
    }

    static double GetLength(int[] path, double[,] D)
    {
        double length = 0;
        for (int i = 0; i < N - 1; i++)
        {
            int j = path[i];
            int k = path[i + 1];
            length += D[j, k];
        }
        length += D[path[N - 1], path[0]];
        return length;
    }
}

class HopfieldNetwork
{
    private int N; // Number of neurons
    private double[,] W; // Weight matrix
    private int[] S; // State of the neurons

    public HopfieldNetwork(int N)
    {
        this.N = N;
        this.W = new double[N, N];
        this.S = new int[N];
    }

    public void SetInput(int[] input)
    {
        for (int i = 0; i < N; i++)
        {
            S[i] = input[i];
        }
    }

    public int[] GetOutput()
    {
        return S;
    }

    public void Update()
    {
        for (int i = 0; i < N; i++)
        {
            double sum = 0;
            for (int j = 0; j < N; j++)
            {
                sum += W[i, j] * S[j];
            }
            if (sum > 0)
            {
                S[i] = 1;
            }
            else if (sum < 0)
            {
                S[i] = -1;
            }
        }
    }
}