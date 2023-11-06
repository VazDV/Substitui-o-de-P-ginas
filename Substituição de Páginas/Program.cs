using System;
using System.Linq;
using System.Runtime.Intrinsics.Arm;

class Program
{
    static void Main(string[] args)
    {
        int[,] ram = new int[10, 6];
        int[,] swap = new int[100, 6];

        // Preencha a matriz SWAP
        Random random = new Random();
        for (int i = 0; i < 100; i++)
        {
            swap[i, 0] = i; // N
            swap[i, 1] = i + 1; // I
            swap[i, 2] = random.Next(1, 51); // D
            swap[i, 3] = 0; // R
            swap[i, 4] = 0; // M
            swap[i, 5] = random.Next(100, 10000); // T
        }

        // Preencha a matriz RAM com páginas da SWAP
        for (int i = 0; i < 10; i++)
        {
            int randomIndex = random.Next(0, 100);
            for (int j = 0; j < 6; j++)
            {
                ram[i, j] = swap[randomIndex, j];
            }
        }

        // Inicialize as variáveis para os algoritmos
        int nruInstructionCount = 0;
        int fifoInstructionCount = 0;
        int fifoScInstructionCount = 0;
        int relogioInstructionCount = 0;
        int wsClockInstructionCount = 0;

        ImprimirMatriz("MATRIZ RAM no início:", ram);
        ImprimirMatriz("MATRIZ SWAP no início:", swap);
        // Execute as instruções
        for (int instruction = 1; instruction <= 5001; instruction++)
        {
            int pageIndex = random.Next(0, 99);
            int ramIndex = -1;

            // Verifique se a página está na RAM
            for (int i = 0; i < 10; i++)
            {
                if (ram[i, 1] == pageIndex + 1)
                {
                    ramIndex = i;
                    break;
                }
            }

            // Execute as operações de acordo com o algoritmo
            if (ramIndex != -1)
            {
                DoInstruction(ref swap, ref ram, ref ramIndex);
            }
            else
            {
                //Substituir a página de acordo com o algoritmo
                if (nruInstructionCount < 1000)
                {
                    NRU(ref swap, ref ram, ref pageIndex, ref nruInstructionCount, ref ramIndex);
                    DoInstruction(ref swap, ref ram, ref ramIndex);
                }
                else if (fifoInstructionCount < 1000)
                {
                    FIFO(ref swap, ref ram, ref pageIndex, ref fifoInstructionCount, ref ramIndex);
                    DoInstruction(ref swap, ref ram, ref ramIndex);
                }
                else if (fifoScInstructionCount < 1000)
                {
                    FIFO_SC(ref swap, ref ram, ref pageIndex, ref fifoScInstructionCount, ref ramIndex);
                    DoInstruction(ref swap, ref ram, ref ramIndex);
                }
                if (relogioInstructionCount < 1000)
                {
                    RELOGIO(ref swap, ref ram, ref pageIndex, ref relogioInstructionCount, ref ramIndex);
                    DoInstruction(ref swap, ref ram, ref ramIndex);
                }
                if (wsClockInstructionCount < 1000)
                {
                    WSCLOCK(ref swap, ref ram, ref pageIndex, ref wsClockInstructionCount, ref ramIndex);
                    DoInstruction(ref swap, ref ram, ref ramIndex);
                }
            }

            // Zerar o Bit R a cada 10 instruções
            if (instruction % 10 == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    ram[i, 3] = 0; // Zerar o Bit R
                }
            }

            // Salvar páginas modificadas na SWAP
            for (int i = 0; i < 10; i++)
            {
                if (ram[i, 4] == 1)
                {
                    // Atualize a SWAP e zere o Bit M
                    for (int j = 0; j < 100; j++)
                    {
                        if (swap[j, 1] == ram[i, 1])
                        {
                            swap[j, 2] = ram[i, 2];
                            swap[j, 4] = 0; // Zerar o Bit M
                            break;
                        }
                    }
                }
            }
            ramIndex = -1;
        }

        // Imprimir as MATRIZES RAM e SWAP no final
        ImprimirMatriz("MATRIZ RAM no final:", ram);
        ImprimirMatriz("MATRIZ SWAP no final:", swap);

        Console.WriteLine("\n\nNRU: " + nruInstructionCount);
        Console.WriteLine("\n\nFIFO: " + fifoInstructionCount);
        Console.WriteLine("\n\nFIFOSC: " + fifoScInstructionCount);
        Console.WriteLine("\n\nRELOGIO: " + relogioInstructionCount);
        Console.WriteLine("\n\nWSRELOGIO: " + wsClockInstructionCount);

    }

    static void ImprimirMatriz(string mensagem, int[,] matriz)
    {
        Console.WriteLine(mensagem);
        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                Console.Write(matriz[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    static void NRU(ref int[,] swap, ref int[,] ram, ref int pageIndex, ref int instructionCount, ref int ramIndex)
    {
        bool substituido = false;
        int i = 0;
        while (substituido == false)
        {
            if (i != 10)
            {
                if (ram[i, 3] == 0)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        ram[i, j] = swap[pageIndex, j];
                    }
                    substituido = true;
                    ramIndex = i;
                }
                else
                {
                    ram[i, 3] = 0;
                    i++;
                }
            }
            else
            {
                i = 0;
            }      
        }
        // Atualizar instructionCount
        instructionCount++;
    }

    static void FIFO(ref int[,] swap, ref int[,] ram, ref int pageIndex, ref int instructionCount, ref int ramIndex)
    {
        //Retirando o primeiro elemento
        for (int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                if (i == 9)
                {
                    ram[9, j] = swap[pageIndex, j]; //Acrescentando o último
                }
                else
                {
                    ram[i, j] = ram[i + 1, j];
                }                
            }
        }
        ramIndex = 0;
        instructionCount++;
    }

    static void FIFO_SC(ref int[,] swap, ref int[,] ram, ref int pageIndex, ref int instructionCount, ref int ramIndex)
    {
        bool substituido = false;
        while(substituido == false)
        {
            if(ram[0, 3] == 0)
            {
                substituido = true;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (i == 9)
                        {
                            ram[9, j] = swap[pageIndex, j]; //Acrescentando o último
                        }
                        else
                        {
                            ram[i, j] = ram[i + 1, j];
                        }
                    }
                }
            }
            else
            {
                ram[0, 3] = 0;
                int[,] ramaux = new int[1, 6];

                //Salva a primeira pagina
                for (int j = 0; j <  6; j++)
                {
                    ramaux[0, j] = ram[0, j];
                }

                //Anda a fila
                for (int i = 0; i < 9; i++)
                {
                    for(int j = 0; j < 5; j++)
                    {
                        ram[i, j] = ram[i + 1, j];
                    }
                }

                //Coloca a primeira em último
                for (int j = 0; j < 6; j++)
                {
                    ram[9, j] = ramaux[0, j];
                }
            }
        }
        ramIndex = 0;
        instructionCount++;
    }

    static void RELOGIO(ref int[,] swap, ref int[,] ram, ref int pageIndex, ref int instructionCount, ref int ramIndex)
    {
        bool substituido = false;
        int i = 0;
        while (substituido == false)
        {
            if (i != 10)
            {
                if (ram[i, 3] == 0)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        ram[i, j] = swap[pageIndex, j];
                    }
                    substituido = true;
                    ramIndex = i;
                }
                else
                {
                    ram[i, 3] = 0;
                    i++;
                }
            }
            else
            {
                i = 0;
            }
        }
        // Atualizar instructionCount
        instructionCount++;
    }

    static void WSCLOCK(ref int[,] swap, ref int[,] ram, ref int pageIndex, ref int instructionCount, ref int ramIndex)
    {
        bool substituido = false;
        int i = 0;
        Random random = new Random();
        int EP = random.Next(100, 10000);
        while (substituido == false)
        {
            if (i < 10)
            {
                if (ram[i, 3] == 0 && EP <= ram[i, 5])
                {
                    for (int j = 0; j < 6; j++)
                    {
                        ram[i, j] = swap[pageIndex, j];
                    }
                    substituido = true;
                    ramIndex = i;
                }
                else
                {
                    ram[i, 3] = 0;
                    i++;
                }
            }
            else
            {
                for (int k = 0; k < 9; k++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (k == 9)
                        {
                            ram[9, j] = swap[pageIndex, j]; //Acrescentando o último
                        }
                        else
                        {
                            ram[k, j] = ram[k + 1, j];
                        }
                    }
                }
                ramIndex = 0;
                substituido = true;
            }
        }
        // Atualizar instructionCount
        instructionCount++;
    }

    static void DoInstruction(ref int[,] swap, ref int[,] ram, ref int ramIndex)
    {
        Random random = new Random();
        // Operação para páginas na RAM
        ram[ramIndex, 3] = 1; // Atualizar o bit de acesso R

        if (random.Next(1, 101) <= 30)
        {
            ram[ramIndex, 2]++; // Atualizar o campo Dado (D)
            ram[ramIndex, 4] = 1; // Atualizar o bit de Modificação M
        }
    }
}