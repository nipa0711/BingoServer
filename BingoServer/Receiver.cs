using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BingoServer
{
    class Receiver
    {
        NetworkStream NS = null;
        StreamReader SR = null;
        StreamWriter SW = null;
        TcpClient client;

        public void startClient(TcpClient clientSocket)
        {
            client = clientSocket;
            Thread chat_thread = new Thread(listening); // listening 함수를 쓰레드로 사용
            chat_thread.Start(); // 쓰레드 시작
        }              

        public void listening()
        {
            NS = client.GetStream(); // 소켓에서 메시지를 가져오는 스트림
            SR = new StreamReader(NS, Encoding.UTF8); // Get message
            SW = new StreamWriter(NS, Encoding.UTF8); // Send message

            string GetMessage = string.Empty;
            string user = string.Empty;

            while (client.Connected == true) //클라이언트 메시지받기
            {
                try
                {
                    GetMessage = SR.ReadLine();
                    string[] MsgResult = GetMessage.Split('|');

                    user = Program.clientList[client];

                    if (GetMessage == "G00D-BY2") // 정상적인 접속 종료
                    {
                        Console.WriteLine("{0}님이 접속을 종료했습니다. ID : {1} [{2}]", ((IPEndPoint)client.Client.RemoteEndPoint).ToString(), user, DateTime.Now);
                        Program.clientList.Remove(client); // 키 값으로 값 삭제
                        Program.bingoReadyUser.Remove(user);
                        Program.saveUserBingo.Remove(user);
                        if (user == Program.userList[0])
                        {
                            Program.userList[0] = "";
                        }
                        else
                        {
                            Program.userList[1] = "";
                        }
                        Console.WriteLine("현재 유저 수 {0}명 [{1}]", Program.clientList.Count, DateTime.Now);

                        if (SW != null)
                            SW.Close();
                        if (SR != null)
                            SR.Close();
                        if (client != null)
                            client.Close();
                        if (NS != null)
                            NS.Close();

                        if (Program.clientList.Count >= 1)
                        {
                            Program.sendAll("#MSG#|" + user + "님이 접속을 종료했습니다.");
                            Program.sendAll("#GAME-0VER#|");
                            Program.sendAll("#MSG#|" + user + "님의 퇴장으로 승리하셨습니다.");
                        }

                        break;
                    }
                    else if (MsgResult[0] == "#0rder#") // 빙고 번호 교환
                    {
                        Console.WriteLine("Game Log: {0}님이 {1}을 불렀습니다. [{2}]", user, MsgResult[1], DateTime.Now);
                        Program.sendAll(GetMessage);
                        Program.sendAll("#MSG#|" + user + "님이 " + MsgResult[1] + "을 불렀습니다.");

                        GameManager.chkBingo(Convert.ToInt32(MsgResult[1])); // 눌렀는거 체크
                        string go = GameManager.isBingo();

                        if (go == "continue")
                        {
                            // 빙고가 되지 않았다면
                            string first = Program.nextUser;

                            TcpClient send = Program.bingoReadyUser[first];
                            NetworkStream stream = send.GetStream();
                            StreamWriter sendMsg = new StreamWriter(stream, Encoding.UTF8);

                            sendMsg.WriteLine("#Attack#|"); // 메시지 보내기
                            sendMsg.Flush();

                            if (Program.nextUser == Program.userList[1]) // 다음 턴
                            {
                                Program.nextUser = Program.userList[0];
                            }
                            else
                            {
                                Program.nextUser = Program.userList[1];
                            }
                        }
                    }
                    else if (MsgResult[0] == "#ready#") // 유저의 빙고 준비완료
                    {
                        Console.WriteLine("{0}님이 준비되었습니다. [{1}]", user, DateTime.Now);
                        Program.sendAll("#MSG#|" + user + "님이 준비되었습니다.");

                        Program.saveUserBingo.Add(user, MsgResult[1]); // 빙고 복사
                        Program.bingoReadyUser.Add(user, client); // 빙고가 준비완료된 유저

                        if (Program.saveUserBingo.Count >= 2) // 준비된 인원이 2명이상이면
                        {
                            // 순서를 랜덤으로 결정한다.
                            Console.WriteLine("순서를 결정합니다.");
                            Program.sendAll("#MSG#|" + "순서를 결정 중 입니다.");

                            GameManager.whoFirst();
                        }

                    }
                    else if (MsgResult[0] == "#MSG#") // 채팅
                    {
                        Program.sendAll(GetMessage);
                        Console.WriteLine("Chat Log: {0} - {1} [{2}]", ((IPEndPoint)client.Client.RemoteEndPoint).ToString(), MsgResult[1], DateTime.Now);
                    }
                }
                catch (Exception ee) // 비정상적인 오류
                {
                    Program.clientList.Remove(client); // 키 값으로 값 삭제
                    Program.bingoReadyUser.Remove(user);
                    Program.saveUserBingo.Remove(user);
                    if (user == Program.userList[0])
                    {
                        Program.userList[0] = "";
                    }
                    else
                    {
                        Program.userList[1] = "";
                    }

                    Console.WriteLine("{0}님이 접속을 종료했습니다. ID : {1} [{2}]", ((IPEndPoint)client.Client.RemoteEndPoint).ToString(), user, DateTime.Now);
                    Console.WriteLine("현재 유저 수 {0}명 [{1}]", Program.clientList.Count, DateTime.Now);

                    if (Program.clientList.Count >= 1)
                    {
                        Program.sendAll("#MSG#|" + user + "님이 접속을 종료했습니다.");
                    }

                    if (SW != null)
                        SW.Close();
                    if (SR != null)
                        SR.Close();
                    if (client != null)
                        client.Close();
                    if (NS != null)
                        NS.Close();

                    Console.WriteLine("===================================");
                    Console.WriteLine(ee.ToString());
                    Console.WriteLine("===================================");
                }
            }
        }
    }
}
