using System.Threading.Channels;

namespace OptimizationMethods;

public class Task_2
{
    #region CONST

    private readonly int a0;
    private readonly int b0;
    private readonly double g1 = 0;
    private readonly double g2 = 0;

    private readonly int[] x = new[] {1, 2, 3, 4, 5};
    private readonly int[] y = new[] {-1, -2, -4, 0, 5};

    private const double epsilon = 1e-4;

    #endregion
    
    public Task_2()
    {
        a0 = Random.Shared.Next(0, 100);
        b0 = Random.Shared.Next(0, 100);

        g1 = Random.Shared.NextDouble();
        g2 = Math.Sqrt(1d - g1 * g1);

        Console.WriteLine($"g1 = {g1}, g2 = {g2}, e = {epsilon}");
    }

    #region F
    
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
    
    private double F_3(double a, double b) {
        double result = 0;
        var F3_max = Math.Abs(a * x[0] + b - y[0]);
        for (var i = 1; i < 5; i++) {
            result = Math.Abs(a * x[i] + b - y[i]);
            if (result > F3_max) {
                F3_max = result;
            }
        }
        return F3_max;
    }
    
    private int F_3_index(double a, double b) {
        double result = 0;
        var k = 0;
        var F3_max = Math.Abs(a * x[0] + b - y[0]);
        for (var i = 1; i < 5; i++) {
            result = Math.Abs(a * x[i] + b - y[i]);
            if (result > F3_max) {
                F3_max = result;
                k = i;
            }
        }
        return k;
    }
    
    #endregion
    
    #region Phi
    
    private double Phi_1(double gamma) => F_1(a0 + gamma * g1, b0 + gamma * g2);
    private double Phi_2(double gamma) => F_2(a0 + gamma * g1, b0 + gamma * g2);
    private double Phi_3(double gamma) => F_3(a0 + gamma * g1, b0 + gamma * g2);
    
    #endregion

    #region Derivatives
    
    private double DerivativeValuePhi_1(double gamma)
    {
        double phi1_d = 0;
        for (int i = 0; i < 5; i++) {
            phi1_d += 2 * (a0 * x[i] + b0 - y[i] + gamma * (g1 * x[i] + g2)) * (g1 * x[i] + g2);
        }
        return phi1_d;
    }

    private double SecondDerivativeValuePhi_1(double gamma)
    {
        return (DerivativeValuePhi_1(gamma + epsilon) - DerivativeValuePhi_1(gamma)) / epsilon;
    }

    private double DerivativeValuePhi_2(double gamma)
    {
        double phi2_d = 0;
        for (int i = 0; i < 5; i++) {
            phi2_d += (a0 * x[i] + b0 - y[i] + gamma * (g1 * x[i] + g2)) * (g1 * x[i] + g2) / 
                      (Math.Abs (a0 * x[i] + b0 - y[i] + gamma * (g1 * x[i] + g2)));
        }
        return phi2_d;
    }

    private double DerivativeValuePhi_3(double gamma)
    {
        var k = F_3_index(a0 + gamma * g1, b0 + gamma * g2);
        var phi3_d = (a0 * x[k] + b0 - y[k] + gamma * (g1 * x[k] + g2)) * (g1 * x[k] + g2) / 
                        Math.Abs(a0 * x[k] + b0 - y[k] + gamma * (g1 * x[k] + g2));
        return phi3_d;
    }
    
    #endregion

    #region LipschitzConstant
    
    private double LipschitzConstantForPhi_1()
    {
        return Math.Abs(Math.Abs(DerivativeValuePhi_1(-10)) > Math.Abs(DerivativeValuePhi_1(10)) ? 
            DerivativeValuePhi_1(-10) : DerivativeValuePhi_1(10));
    }

    private double LipschitzConstantForPhi_2()
    {
        double a = -10, b = 10;
        while ((b - a) / 2 > epsilon)
        {
            var c = (a + b - epsilon) / 2;
            var d = (a + b + epsilon) / 2;
            if (Math.Abs(DerivativeValuePhi_2(c)) > Math.Abs(DerivativeValuePhi_2(d))) {
                a = c;
            }
            else
                b = d;
        }
        return Math.Abs(DerivativeValuePhi_2((a + b) / 2));
    }

    private double LipschitzConstantForPhi_3()
    {
        double a = -10, b = 10;
        while ((b - a) / 2 > epsilon)
        {
            var c = (a + b - epsilon) / 2;
            var d = (a + b + epsilon) / 2;
            if (Math.Abs(DerivativeValuePhi_3(c)) > Math.Abs(DerivativeValuePhi_3(d))) {
                a = c;
            }
            else
                b = d;
        }
        return Math.Abs(DerivativeValuePhi_3((a + b) / 2));
    }
    
    #endregion

    private double PolylineMethod(double constant, Func<double, double> phi)
    {
        double a = -10, b = 10;
        var xList = new List<double>();
        var p = new List<double>();
        var pmin = 0;
        var x1 = (phi(a) - phi(b) + constant * (a + b)) / (2 * constant);
        var p1 = 0.5 * (phi(a) + phi(b) + constant * (a - b));
        // Instead of a pair of numbers (x1, p1), we form pairs (x2, p2), (x3, p3)
        var delta = (phi(x1) - p1) / (2  * constant);
        var p2 = 0.5 * (phi(x1) + p1);
        var x2 = x1 - delta;
        var x3 = x1 + delta;
        p.Add(p2);
        p.Add(p2);
        xList.Add(x2);
        xList.Add(x3);

        var i = 0;
        while (2  * constant * delta > epsilon) {
            var min = 10e7;
            //From the pairs obtained, we will choose the one with the second component being minimal
            for (i = 0; i < p.Count; i++) {
                if (p[i] <= min) {
                    min = p[i];
                    pmin = i;
                }
            }
            x1 = x[pmin];
            p1 = p[pmin];
            delta = (phi(x1) - p1) / (2 * constant);
            if (2 * constant * delta > epsilon)
                break;
            x2 = x1 - delta;
            x3 = x1 + delta;
            p2 = 0.5 * (phi(x1) + p1);
            xList[pmin] = x2;
            xList.Add(x3);
            p[pmin] = p2;
            p.Add(p2);
        }

        return xList[pmin];
    }

    // only for phi_1
    private void NewtonRaphsonMethod()
    {
        double l = (-10 + 10) / 2;
        while (Math.Abs(DerivativeValuePhi_1(l)) >= 10e-4) {
            l -= DerivativeValuePhi_1(l) / SecondDerivativeValuePhi_1(l);
        }

        Console.WriteLine("Newton Raphson");
        Console.WriteLine($"x = {l}, y = {Phi_1(l)}");
    }

    public void Execute()
    {
        Console.WriteLine(PolylineMethod(LipschitzConstantForPhi_1(), Phi_1));
        Console.WriteLine($"Constant for phi 1 = {LipschitzConstantForPhi_1()}");
        Console.WriteLine($"Constant for phi 2 = {LipschitzConstantForPhi_2()}");
        Console.WriteLine($"Constant for phi 3 = {LipschitzConstantForPhi_3()}");
        NewtonRaphsonMethod();
    }
}