using System;

namespace OptimizationMethods;

public class Task_1
{
    #region CONST

    private readonly int a0;
    private readonly int b0;
    private readonly double g1 = 0;
    private readonly double g2 = 0;

    private readonly int[] x = new[] {1, 2, 3, 4, 5};
    private readonly int[] y = new[] {-1, -2, -4, 0, 5};

    private const double epsilon = 1e-6;
    private const double delta = 1e-7;

    #endregion

    public Task_1()
    {
        a0 = Random.Shared.Next(0, 100);
        b0 = Random.Shared.Next(0, 100);

        g1 = Random.Shared.NextDouble();
        g2 = Math.Sqrt(1d - g1 * g1);

        Console.WriteLine($"g1 = {g1}, g2 = {g2}, e = {epsilon}");
    }

    private double F_1(double a, double b)
    {
        var result = 0d;
        for (var k = 0; k < 5; ++k)
        {
            result += (a * x[k] + b - y[k]) * (a * x[k] + b - y[k]);
        }

        return result;
    }

    private double F_2(double a, double b)
    {
        var result = 0d;
        for (var k = 0; k < 5; ++k)
        {
            result += Math.Abs(a * x[k] + b - y[k]);
        }

        return result;
    }

    private double Phi_1(double gamma) => F_1(a0 + gamma * g1, b0 + gamma * g2);
    private double Phi_2(double gamma) => F_2(a0 + gamma * g1, b0 + gamma * g2);

    private (double, double) Dichotomy(Func<double, double> phi)
    {
        var a = -10d;
        var b = 10d;

        var l = (b - a) / 2;
        
        while (l > epsilon)
        {
            var c = (a + b) / 2 - delta;
            var d = (a + b) / 2 + delta;

            if (phi(c) <= phi(d))
                b = d;
            else
                a = c;

            l = (b - a) / 2;
        }

        return (a, b);
    }

    private (double, double) GoldenRatio(Func<double, double> phi)
    {
        var a = -10d;
        var b = 10d;

        while ((b - a) / 2 > epsilon)
        {
            var c = a + (3 - Math.Sqrt(5)) * (b - a) / 2;
            var d = a + (Math.Sqrt(5) - 1) * (b - a) / 2;

            if (phi(c) <= phi(d))
            {
                b = d;
                d = c;
                c = a + (3 - Math.Sqrt(5)) * (b - a) / 2;
            }
            else
            {
                a = c;
                c = d;
                d = a + (Math.Sqrt(5) - 1) * (b - a) / 2;
            }
        }

        return (a, b);
    }

    public void Execute()
    {
        var (a, b) = Dichotomy(Phi_1);
        Console.WriteLine($"a0 = {a0}, b0 = {b0}, en = {(b0 - a0) / 2}, c_n = {a}, d_n = {b}, " +
                          $"phi_1(c) = {Phi_1(a)}, phi_1(d) = {Phi_1(b)}");
        
        (a, b) = Dichotomy(Phi_2);
        Console.WriteLine($"a0 = {a0}, b0 = {b0}, en = {(b0 - a0) / 2}, c_n = {a}, d_n = {b}, " +
                          $"phi_2(c) = {Phi_2(a)}, phi_2(d) = {Phi_2(b)}");
        
        (a, b) = GoldenRatio(Phi_1);
        Console.WriteLine($"a0 = {a0}, b0 = {b0}, en = {(b0 - a0) / 2}, c_n = {a}, d_n = {b}, " +
                          $"phi_1(c) = {Phi_1(a)}, phi_1(d) = {Phi_1(b)}");
        
        (a, b) = GoldenRatio(Phi_2);
        Console.WriteLine($"a0 = {a0}, b0 = {b0}, en = {(b0 - a0) / 2}, c_n = {a}, d_n = {b}, " +
                          $"phi_2(c) = {Phi_2(a)}, phi_2(d) = {Phi_2(b)}");
    }
}