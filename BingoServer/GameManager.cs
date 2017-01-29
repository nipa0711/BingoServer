using System;

namespace BingoServer
{
    class GameManager
    {
        public static void copyBingo(string USER) // 유저의 빙고를 복사한다.
        {
            Program.playerA_bingoCount = 0;
            Program.playerB_bingoCount = 0;

            if (USER == Program.userList[0])
            {
                string msg1 = Program.saveUserBingo[Program.userList[0]]; // userList[0]의 메세지를 갖고 온다.
                string msg2 = Program.saveUserBingo[Program.userList[1]]; // userList[1]의 메세지를 갖고 온다.

                Program.playerA = Program.userList[0];
                Program.playerB = Program.userList[1];

                string[] MsgBingo1 = msg1.Split(',');
                string[] MsgBingo2 = msg2.Split(',');

                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        Program.bingoA[i, j] = Convert.ToInt32(MsgBingo1[count]);
                        Program.bingoB[i, j] = Convert.ToInt32(MsgBingo2[count]);
                        count++;
                    }
                }
            }
            else
            {
                string msg1 = Program.saveUserBingo[Program.userList[1]]; // userList[1]의 메세지를 갖고 온다.
                string msg2 = Program.saveUserBingo[Program.userList[0]]; // userList[0]의 메세지를 갖고 온다.

                Program.playerA = Program.userList[1];
                Program.playerB = Program.userList[0];

                string[] MsgBingo1 = msg1.Split(',');
                string[] MsgBingo2 = msg2.Split(',');

                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        Program.bingoA[i, j] = Convert.ToInt32(MsgBingo1[count]);
                        Program.bingoB[i, j] = Convert.ToInt32(MsgBingo2[count]);
                        count++;
                    }
                }
            }
        }

        public static void chkBingo(int number)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (Program.bingoA[i, j] == number)
                    {
                        Program.bingoA[i, j] = 0;
                    }

                    if (Program.bingoB[i, j] == number)
                    {
                        Program.bingoB[i, j] = 0;
                    }
                }
            }
        }

        public static string isBingo()
        {
            int i, j;
            int playerA_rowsFlag = 0;
            int playerA_columnFlag = 0;
            int playerA_crossleftFlag = 0;
            int playerA_crossrightFlag = 0;
            int playerA_check = 0;

            int playerB_rowsFlag = 0;
            int playerB_columnFlag = 0;
            int playerB_crossleftFlag = 0;
            int playerB_crossrightFlag = 0;
            int playerB_check = 0;

            for (i = 0; i < 5; i++)
            {
                playerA_rowsFlag = 0;
                playerA_columnFlag = 0;

                playerB_rowsFlag = 0;
                playerB_columnFlag = 0;

                for (j = 0; j < 5; j++)
                {
                    if (Program.bingoA[i, j] == 0)
                    {
                        playerA_rowsFlag++;
                    }
                    if (Program.bingoA[j, i] == 0)
                    {
                        playerA_columnFlag++;
                    }

                    if (Program.bingoB[i, j] == 0)
                    {
                        playerB_rowsFlag++;
                    }
                    if (Program.bingoB[j, i] == 0)
                    {
                        playerB_columnFlag++;
                    }
                }
                // 가로체크
                if (playerA_rowsFlag == 5)
                {
                    playerA_check++;
                }
                // 세로체크
                if (playerA_columnFlag == 5)
                {
                    playerA_check++;
                }
                // 대각선 왼쪽에서 오른쪽
                if (Program.bingoA[i, i] == 0)
                {
                    playerA_crossleftFlag++;
                }
                // 대각선 오른쪽에서 왼쪽
                if (Program.bingoA[5 - 1 - i, i] == 0)
                {
                    playerA_crossrightFlag++;
                }

                // 가로체크
                if (playerB_rowsFlag == 5)
                {
                    playerB_check++;
                }
                // 세로체크
                if (playerB_columnFlag == 5)
                {
                    playerB_check++;
                }
                // 대각선 왼쪽에서 오른쪽
                if (Program.bingoB[i, i] == 0)
                {
                    playerB_crossleftFlag++;
                }
                // 대각선 오른쪽에서 왼쪽
                if (Program.bingoB[5 - 1 - i, i] == 0)
                {
                    playerB_crossrightFlag++;
                }
            }

            if (playerA_crossleftFlag == 5)
            {
                playerA_check++;
            }

            if (playerA_crossrightFlag == 5)
            {
                playerA_check++;
            }

            if (playerB_crossleftFlag == 5)
            {
                playerB_check++;
            }

            if (playerB_crossrightFlag == 5)
            {
                playerB_check++;
            }

            if (playerA_check != Program.playerA_bingoCount)
            {
                Console.WriteLine("Game Log: {0}님의 {1}BINGO! [{2}]", Program.playerA, playerA_check, DateTime.Now);
                Program.playerA_bingoCount++;
                Program.sendAll("#MSG#|" + Program.playerA + "님의 " + playerA_check + " BINGO! ====");
            }

            if (playerB_check != Program.playerB_bingoCount)
            {
                Console.WriteLine("Game Log: {0}님의 {1}BINGO! [{2}]", Program.playerB, playerB_check, DateTime.Now);
                Program.playerB_bingoCount++;
                Program.sendAll("#MSG#|" + Program.playerB + "님의 " + playerB_check + " BINGO! ====");
            }

            if (playerA_check >= 3 && playerB_check < 3)
            {
                Console.WriteLine("Game Log: {0}님이 승리했습니다. [{1}]", Program.playerA, DateTime.Now);
                Program.sendAll("#MSG#|" + Program.playerA + "님의 승리!!");
                roundClear();
                return "finish";
            }
            else if (playerB_check >= 3 && playerA_check < 3)
            {
                Console.WriteLine("Game Log: {0}님이 승리했습니다. [{1}]", Program.playerB, DateTime.Now);
                Program.sendAll("#MSG#|" + Program.playerB + "님의 승리!!");
                roundClear();
                return "finish";
            }
            else if (playerA_check >= 3 && playerB_check >= 3)
            {
                Console.WriteLine("Game Log: {0}님 {1}님의 공동 승리입니다. [{2}]", Program.playerA, Program.playerB, DateTime.Now);
                Program.sendAll("#MSG#|" + Program.playerA + ", " + Program.playerB + "님의 공동 승리!!");
                roundClear();
                return "finish";
            }
            return "continue";
        }

        public static void roundClear()
        {
            Program.sendAll("#GAME-0VER#|");
            Program.bingoReadyUser.Remove(Program.playerA);
            Program.saveUserBingo.Remove(Program.playerA);
            Program.bingoReadyUser.Remove(Program.playerB);
            Program.saveUserBingo.Remove(Program.playerB);
        }
    }
}
