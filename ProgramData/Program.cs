public class array
{


public static void Main(string[] args)

{
    Console.WriteLine("## 1.Едномерен масив");
 
 int[] numbers = { 15, 7, 22, 11, 7, 30, 5, 22, 7, 18 };

Console.WriteLine("\nМасив: [ " + string.Join(", ", numbers) + " ]");

Console.WriteLine("Елемент на индекс 0: " + numbers[0]); // Първи елемент (15)
        Console.WriteLine("Елемент на индекс 4: " + numbers[4]); // Пети елемент (7)
        Console.WriteLine("Последният елемент (индекс " + (numbers.Length - 1) + "): " + numbers[numbers.Length - 1]); // (18)
  

}





}