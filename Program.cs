using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_06
{
    class Program
    {
        static void Main()
        {

            /// Домашнее задание
            ///
            /// Группа начинающих программистов решила поучаствовать в хакатоне с целью демонстрации
            /// своих навыков. 
            /// 
            /// Немного подумав они вспомнили, что не так давно на занятиях по математике
            /// они проходили тему "свойства делимости целых чисел". На этом занятии преподаватель показывал
            /// пример с использованием фактов делимости. 
            /// Пример заключался в следующем: 
            /// Написав на доске все числа от 1 до N, N = 50, преподаватель разделил числа на несколько групп
            /// так, что если одно число делится на другое, то эти числа попадают в разные руппы. 
            /// В результате этого разбиения получилось M групп, для N = 50, M = 6
            /// 
            /// N = 50
            /// Группы получились такими: 
            /// 
            /// Группа 1: 1
            /// Группа 2: 2 3 5 7 11 13 17 19 23 29 31 37 41 43 47
            /// Группа 3: 4 6 9 10 14 15 21 22 25 26 33 34 35 38 39 46 49
            /// Группа 4: 8 12 18 20 27 28 30 42 44 45 50
            /// Группа 5: 16 24 36 40
            /// Группа 6: 32 48
            /// 
            /// M = 6
            /// 
            /// ===========
            /// 
            /// N = 10
            /// Группы получились такими: (не совпадает)
            /// 
            /// Группа 1: 1
            /// Группа 2: 2 7 9
            /// Группа 3: 3 4 10  (3 можно было поместить во 2 группу, т.к на 2 она не делится, группы будут по-другому наполнены)
            /// Группа 4: 5 6 8
            /// 
            /// M = 4
            /// 
            /// Участники хакатона решили эту задачу, добавив в неё следующие возможности:
            /// 1. Программа считыват из файла (путь к которому можно указать) некоторое N, 
            ///    для которого нужно подсчитать количество групп
            ///    Программа работает с числами N не превосходящими 1 000 000 000
            ///   
            /// 2. В ней есть два режима работы:
            ///   2.1. Первый - в консоли показывается только количество групп, т е значение M
            ///   2.2. Второй - программа получает заполненные группы и записывает их в файл используя один из
            ///                 вариантов работы с файлами
            ///            
            /// 3. После выполения пунктов 2.1 или 2.2 в консоли отображается время, за которое был выдан результат 
            ///    в секундах и миллисекундах
            /// 
            /// 4. После выполнения пунта 2.2 программа предлагает заархивировать данные и если пользователь соглашается -
            /// делает это.
            /// 
            /// Попробуйте составить конкуренцию начинающим программистам и решить предложенную задачу
            /// (добавление новых возможностей не возбраняется)
            ///
            /// * При выполнении текущего задания, необходимо документировать код 
            ///   Как пометками, так и xml документацией
            ///   В обязательном порядке создать несколько собственных методов


            Console.WriteLine("Введите путь файла и нажмите Enter. В случае отсутствия ввода файл будет считан файл inputfile.txt из текущей директории");
            string path = Console.ReadLine();
            int number = ReadNumberFromFile(path);

            if (number == -1)
            {
                Console.WriteLine("Содержимое файла имеет неверный формат. Нажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Считанный номер N = {number}");
            Console.WriteLine($"Выберите режим работы:\n1) Отобразить количество групп в консоли\n2) Записать полученные группы в файл");
            int mode = int.Parse(Console.ReadLine());

            DateTime startTime = DateTime.Now;
            TimeSpan deltaTime;

            if (mode == 1)
            {
                Console.WriteLine(GetGroupsCount(number));
                deltaTime = DateTime.Now - startTime;
            }
            else if (mode == 2)
            {
                WriteGropsInFile(number);
                deltaTime = DateTime.Now - startTime;
                Console.WriteLine("Хотите архивировать файл?\n1)Да \n2)Нет");
                int des = int.Parse(Console.ReadLine());
                if (des == 1)
                {
                    Archive();
                }
            }
            else
            {
                Console.WriteLine("Введенные данные имеют неверный формат. Нажмите любую клавишу для выхода...");
                return;
            }

            Console.WriteLine($"Операция заняла {deltaTime.TotalMilliseconds} милисекунд");
            Console.ReadKey();

        }

        /// <summary>
        /// Записть в файл групп, составленных из данного числа N
        /// </summary>
        /// <param name="number">Число N</param>
        public static void WriteGropsInFile(int number)
        {
            
            using (StreamWriter streamWriter = new StreamWriter("outputfile.txt"))
            {
                for (int i = 1; i <= GetGroupsCount(number); i++)
                {
                    for (int j = (int)Math.Pow(2, i - 1); j < Math.Pow(2, i); j++) 
                    {
                        if (j <= number)
                        {
                            streamWriter.Write($"{j} ");
                        }
                        else
                        {
                            break;
                        }
                    }
                    streamWriter.WriteLine();
                }
            }
        }

        /// <summary>
        /// Архивирует файл output.txt, создает файл outputcompressed
        /// </summary>
        public static void Archive()
        {
            using (FileStream reader = new FileStream("outputfile.txt", FileMode.Open))
            {
                using (FileStream writer = new FileStream("outputcompressed", FileMode.Create))
                {
                    using (GZipStream zip = new GZipStream(writer, CompressionMode.Compress))
                    {
                        reader.CopyTo(zip);
                        Console.WriteLine($"Сжатие завершено. Было {reader.Length} стало {writer.Length}");
                    }
                }
            }
        }

        /// <summary>
        /// Извлечение числа из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>int если содержимое файла имеет верный формат, -1 если нет</returns>
        public static int ReadNumberFromFile(string path)
        {
            if (!path.Any())
            {
                path = Directory.GetCurrentDirectory() + @"\inputfile.txt";
            }

            string filetext = File.ReadAllText(path);

            if (int.TryParse(filetext, out int n) && n > 0 && n < 1_000_000_000)
            {
                return n;
            }
            else
            {
                return -1;
            }
        }        

        /// <summary>
        /// Получение количества групп числа N
        /// </summary>
        /// <param name="number">Число N</param>
        /// <returns></returns>
        public static int GetGroupsCount(int number)
        {
            int groupcount = 1;
            while (number / 2 != 0)
            {
                number /= 2;
                groupcount++;
            }
            return groupcount;
        }

        
       
        
        
    }
}

