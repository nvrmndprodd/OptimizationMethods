namespace OptimizationMethods;

public class Task_3
{
    #region CONST

    private readonly int a0;
    private readonly int b0;

    private readonly int[] x = new[] {1, 2, 3, 4, 5};
    private readonly int[] y = new[] {-1, -2, -4, 0, 5};

    private const double epsilon = 1e-6;

    #endregion
    
    public Task_3()
    {
        a0 = Random.Shared.Next(0, 100);
        b0 = Random.Shared.Next(0, 100);
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
    
    #region Coordinate Descent
    // Минимизируем функцию по первой координате методом золотого сечения на отрезке
    // [-1000, 1000]
    private double MinimizeFirstCoordinate(double secondCoordinate)
    {
        double a = -1000;
        double b = 1000;
        var c = (3 - Math.Sqrt(5)) / 2 * (b - a) + a;
        var d = a + (Math.Sqrt(5) - 1) / 2 * (b - a);

        var F_c = F_1(c, secondCoordinate);
        var F_d = F_1(d, secondCoordinate);

        while (b - a >= epsilon)
        {
            F_c = F_1(c, secondCoordinate);
            F_d = F_1(d, secondCoordinate);

            if (F_c <= F_d)
                b = d;
            else
                a = c;
            
            c = (3 - Math.Sqrt(5)) / 2 * (b - a) + a;
            d = a + (Math.Sqrt(5) - 1) / 2 * (b - a);
        }

        return (a + b) / 2;
    }

    private double MinimizeSecondCoordinate(double firstCoordinate)
    {
        double a = -1000;
        double b = 1000;
        var c = (3 - Math.Sqrt(5)) / 2 * (b - a) + a;
        var d = a + (Math.Sqrt(5) - 1) / 2 * (b - a);

        var F_c = F_1(firstCoordinate, c);
        var F_d = F_1(firstCoordinate, d);

        while (b - a >= epsilon)
        {
            F_c = F_1(firstCoordinate, c);
            F_d = F_1(firstCoordinate, d);

            if (F_c <= F_d)
                b = d;
            else
                a = c;
            
            c = (3 - Math.Sqrt(5)) / 2 * (b - a) + a;
            d = a + (Math.Sqrt(5) - 1) / 2 * (b - a);
        }

        return (a + b) / 2;
    }

    private void CoordinateDescent()
    {
        double x1 = 0, y1 = 0, x = 100, y = 100;
        var n = 0;

        // Пока модуль разности значений функции в точках (x1, y1) и (x, y)
        // не станет меньше эпсилон
        // или расстояние между точками не станет меньше эпсилон
        while (Math.Abs(F_1(x, y) - F_1(x1, y1)) >= epsilon &&
               Math.Sqrt(Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2)) >= epsilon)
        {
            x1 = x;
            y1 = y;

            x = MinimizeFirstCoordinate(y); // фиксируем вторую координату и минимизируем первую
            y = MinimizeSecondCoordinate(x);

            Console.WriteLine($"({x}, {y}), " +
                              $"{Math.Sqrt(Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2))}," +
                              $"{F_1(x, y)}, {Math.Abs(F_1(x, y) - F_1(x1, y1))}");
        }
    }
    
    #endregion
    
    #region Step Fragmentation Gradient

    private double F_1_a(double a, double b)
    {
        var result = 0d;
        for (var k = 0; k < 5; ++k)
        {
            result += x[k] * (a * x[k] + b + y[k]);
        }

        return result;
    }

    private double F_1_b(double a, double b)
    {
        var result = 0d;
        for (var k = 0; k < 5; ++k)
        {
            result += 2 * (a * x[k] + b + y[k]);
        }

        return result;
    }

    private void StepFragmentationGradient()
    {
        var A = 4d;
        var lam = 0.5;

        var xk = new double[] {10, 10};
        var xk_1 = new double[] {xk[0] - A * F_1_a(xk[0], xk[1]), xk[1] - A * F_1_b(xk[0], xk[1])};

        var Fx_1 = F_1(xk_1[0], xk_1[1]);
        var Fxk = F_1(xk[0], xk[1]);

        while (Math.Sqrt(Math.Pow(F_1_a(xk_1[0], xk_1[1]), 2) + Math.Pow(F_1_b(xk_1[0], xk_1[1]), 2)) >= epsilon)
        {
            var ak = A;
            
            while (Fx_1 - Fxk > -ak * epsilon * (Math.Pow(F_1_a(xk_1[0], xk_1[1]), 2) + Math.Pow(F_1_b(xk_1[0], xk_1[1]), 2)))
            {
                ak = ak * lam;
                xk_1 = new double[] {xk[0] - ak * F_1_a(xk[0], xk[1]), xk[1] - ak * F_1_b(xk[0], xk[1])};
                Fx_1 = F_1(xk_1[0], xk_1[1]);
                Fxk = F_1(xk[0], xk[1]);
            }

            xk[0] = xk_1[0];
            xk[1] = xk_1[1];
            
            xk_1 = new double[] {xk[0] - ak * F_1_a(xk[0], xk[1]), xk[1] - ak * F_1_b(xk[0], xk[1])};
            Fx_1 = F_1(xk_1[0], xk_1[1]);
            Fxk = F_1(xk[0], xk[1]);
        }
    }
    
    #endregion

    #region ConstantStepGradiend

    private void ConstantStepGradient()
    {
        var L = 35d;
        var a = (1 - epsilon) / L;
        
        var xk = new double[] {10, 10};
        var xk_1 = new double[] {xk[0] - a * F_1_a(xk[0], xk[1]), xk[1] - a * F_1_b(xk[0], xk[1])};

        var Fx_1 = F_1(xk_1[0], xk_1[1]);
        var Fxk = F_1(xk[0], xk[1]);

        while (Math.Sqrt(Math.Pow(F_1_a(xk_1[0], xk_1[1]), 2) + Math.Pow(F_1_b(xk_1[0], xk_1[1]), 2)) >= epsilon)
        {
            xk[0] = xk_1[0];
            xk[1] = xk_1[1];
            
            xk_1 = new double[] {xk[0] - a * F_1_a(xk[0], xk[1]), xk[1] - a * F_1_b(xk[0], xk[1])};
        }
    }

    #endregion

    #region MNGS

    private double minAlpha(double u, double[] xk, double[] fTemp)
    {
        var a = xk[0] - u * fTemp[0];
        var b = xk[1] - u * fTemp[1];

        return F_1(a, b);
    }
    
    private double GoldenSelectionForAlpha(double[] xk, double[] fTemp)
    {
        double a = -1000;
        double b = 1000;
        var c = (3 - Math.Sqrt(5)) / 2 * (b - a) + a;
        var d = a + (Math.Sqrt(5) - 1) / 2 * (b - a);
        var x = a;

        var F_c = 0d;
        var F_d = 0d;

        while (b - a >= epsilon)
        {
            x = b - a;

            if (F_c <= F_d)
                b = d;
            else
                a = c;
            
            c = (3 - Math.Sqrt(5)) / 2 * (b - a) + a;
            d = a + (Math.Sqrt(5) - 1) / 2 * (b - a);
        }

        return (a + b) / 2;
    }

    private void MNGS()
    {
        var xk = new double[] {0, 0};
        var xk_1 = new double[] {0, 0};

        var a = 0d;
        var fTemp = new double[] {F_1_a(xk[0], xk[1]), F_1_b(xk[0], xk[1])};

        while (Math.Sqrt(Math.Pow(F_1_a(xk_1[0], xk_1[1]), 2) + Math.Pow(F_1_b(xk_1[0], xk_1[1]), 2)) >= epsilon)
        {
            a = GoldenSelectionForAlpha(xk, fTemp);

            xk[0] = xk_1[0];
            xk[1] = xk_1[1];
            
            xk_1 = new double[] {xk[0] - a * F_1_a(xk[0], xk[1]), xk[1] - a * F_1_b(xk[0], xk[1])};
            xk[0] = xk[0] - a * F_1_a(xk[0], xk[1]);
            xk[1] = xk[1] - a * F_1_b(xk[0], xk[1]);

            fTemp = new double[] {F_1_a(xk[0], xk[1]), F_1_b(xk[0], xk[1])};
        }
    }

    #endregion
}