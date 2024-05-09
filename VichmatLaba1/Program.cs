using System.Collections.Generic;
using System.Linq;

class Point
{
    public double x;
    public double y;

    public Point(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
}

class GausseMethod
{

    private static readonly List<Point> points = new List<Point>() { new Point(1.577, 0.427), new Point(1.538, 0.427), new Point(1.333, 0.297 ),
    new Point(1.847, 0.004), new Point(1.797, 0.052), new Point(1.910, -0.098 ), new Point(1.371, 0.565), new Point(1.527, 0.26 ),
    new Point(1.632 , 0.082), new Point(1.034, 0.834) };

    private static int numberOfPoints = points.Count;

    const string LINEAR = "linear";
    const string QUADRATIC = "quadratic";

    static void Main()
    {
        List<double> y = points.Select(p => p.y).ToList();

        double sumX = points.Select(p => p.x).Sum();
        double sumY = points.Select(p => p.y).Sum();
        double sumX2 = points.Select(p => p.x * p.x).Sum();
        double sumXY = points.Select(p => p.x * p.y).Sum();
        double sumX3 = points.Select(p => p.x * p.x * p.x).Sum();
        double sumX4 = points.Select(p => p.x * p.x * p.x * p.x).Sum();
        double sumX2Y = points.Select(p => p.x * p.x * p.y).Sum();


        double[,] matrixLinear = fillMatrixLinear(sumX, sumY, sumX2, sumXY);
        double[,] matrixQuadr = fillMatrixQuadr(sumX, sumY, sumX2, sumXY, sumX3, sumX4, sumX2Y);



        double[] answerLinear = Gauss(matrixLinear);

        double[] answerQuadr = Gauss(matrixQuadr);


        Console.WriteLine("Коэффициенты для линейной аппроксимирующей функции");
        printAnswer(answerLinear, LINEAR);

        Console.WriteLine("Коэффициенты для квадратичной аппроксимирующей функции");
        printAnswer(answerQuadr, QUADRATIC);


        findDeltaLinearFunc(points, answerLinear, LINEAR);

        findDeltaLinearFunc(points, answerQuadr, QUADRATIC);
    }

    private static double[,] fillMatrixLinear(double sumX, double sumY, double sumX2, double sumXY)
    {
        // Задаем систему уравнений в виде матрицы коэффициентов
        double[,] matrix = {
            { sumX2, sumX, sumXY },
            { sumX, numberOfPoints, sumY },
        };

        return matrix;
    }

    private static double[,] fillMatrixQuadr(double sumX, double sumY, double sumX2, double sumXY, double sumX3, double sumX4, double sumX2Y)
    {
        // Задаем систему уравнений в виде матрицы коэффициентов
        double[,] matrix = {
            { sumX2, sumX, numberOfPoints, sumY },
            { sumX3, sumX2, sumX, sumXY },
            { sumX4, sumX3, sumX2, sumX2Y }
        };

        return matrix;
    }

    public static double[] Gauss(double[,] Matrix)
    {
        int n = Matrix.GetLength(0); //Размерность начальной матрицы (строки)
        double[,] Matrix_Clone = new double[n, n + 1]; //Матрица-дублер
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n + 1; j++)
                Matrix_Clone[i, j] = Matrix[i, j];

        // Прямой ход (Зануление нижнего левого угла)
        for (int k = 0; k < n; k++) //k-номер строки
        {
            for (int i = 0; i < n + 1; i++) //i-номер столбца
                Matrix_Clone[k, i] = Matrix_Clone[k, i] / Matrix[k, k]; //Деление k-строки на первый член !=0 для преобразования его в единицу
            for (int i = k + 1; i < n; i++) //i-номер следующей строки после k
            {
                double K = Matrix_Clone[i, k] / Matrix_Clone[k, k]; //Коэффициент
                for (int j = 0; j < n + 1; j++) //j-номер столбца следующей строки после k
                    Matrix_Clone[i, j] = Matrix_Clone[i, j] - Matrix_Clone[k, j] * K; //Зануление элементов матрицы ниже первого члена, преобразованного в единицу
            }
            for (int i = 0; i < n; i++) //Обновление, внесение изменений в начальную матрицу
                for (int j = 0; j < n + 1; j++)
                    Matrix[i, j] = Matrix_Clone[i, j];
        }

        // Обратный ход (Зануление верхнего правого угла)
        for (int k = n - 1; k > -1; k--) //k-номер строки
        {
            for (int i = n; i > -1; i--) //i-номер столбца
                Matrix_Clone[k, i] = Matrix_Clone[k, i] / Matrix[k, k];
            for (int i = k - 1; i > -1; i--) //i-номер следующей строки после k
            {
                double K = Matrix_Clone[i, k] / Matrix_Clone[k, k];
                for (int j = n; j > -1; j--) //j-номер столбца следующей строки после k
                    Matrix_Clone[i, j] = Matrix_Clone[i, j] - Matrix_Clone[k, j] * K;
            }
        }

        // Отделяем от общей матрицы ответы
        double[] Answer = new double[n];
        for (int i = 0; i < n; i++)
            Answer[i] = Matrix_Clone[i, n];

        return Answer;
    }

    private static void printAnswer(double[] answer, string typeOfDependence)
    {
        switch (typeOfDependence)
        {
            case LINEAR:
                Console.WriteLine("a = " + answer[0] + "     b = " + answer[1] + "\n");
                break;

            case QUADRATIC:
                Console.WriteLine("a0 = " + answer[0] + "     a1 = " + answer[1] + "     a2 = " + answer[2] + "\n");
                break;
        }
    
        
    }

    private static void findDeltaLinearFunc(List<Point> points, double[] answer, string typeOfDependence)
    {
        double delta = 0;
        List<double> deltas = new List<double>(points.Count);

        switch (typeOfDependence)
        {

            case LINEAR:
                for (int i = 0; i < points.Count; i++)
                {
                    deltas.Add(points[i].y - (points[i].x * answer[0] + answer[1]));
                    delta += deltas[i];
                }
                break;

            case QUADRATIC:
                for (int i = 0; i < points.Count; i++)
                {
                    deltas.Add(points[i].y - (answer[2] + points[i].x * answer[1]) + points[i].x * points[i].x * answer[0]);
                    delta += deltas[i];
                }
                break;
        }

        for (int i = 0; i < deltas.Count; i++)
        {
            Console.WriteLine("x" + (i+1) + " = " + points[i].x + ", " + "y" + (i+1) + " = " + points[i].y + "    " + "Отклонение точки = " + deltas[i]);
        }

        delta = Math.Sqrt(delta * delta / points.Count);
        Console.WriteLine("Отклонение = " + delta + "\n");

    }

}