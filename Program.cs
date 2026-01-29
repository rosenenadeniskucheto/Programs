using System.Globalization;

public class Program{
public static void Main
(string[] args)
    {
        
    
// new int [x] kogato ne znaem elementite na masiva

int[] numbers = (15 ,3 ,12 ,87 ,1 ,20 , 8);
for (int i = 0; i < numbers.length; i++)
{
    for ( int j = i + 1; j < numbers.Length; j++)
            {
                 if (number[i] < numbers[i + 1])
            {
                int temp = numbers[i+1];
                numbers[i+1] = numbers[i];
                numbers[i] = temp;

            }
                   
            Console.WriteLine(numbers[i]);

            }


    }
  }
}