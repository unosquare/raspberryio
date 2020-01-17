namespace Unosquare.RaspberryIO.Playground.Peripherals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Swan;
    using Unosquare.RaspberryIO.Peripherals;

    public static partial class Peripherals
    {
        private static readonly Dictionary<ConsoleKey, string> RfidOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.D, "Detect Card" },
            { ConsoleKey.W, "Write Card" },
            { ConsoleKey.R, "Read Card" },
            { ConsoleKey.S, "Read Card Sectors" },
            { ConsoleKey.M, "Read Card Sectors (MIFARE Ultralight - no Auth)" },
        };

        public static void ShowRfidMenu()
        {
            var exit = false;

            do
            {
                Console.Clear();
                var mainOption = Terminal.ReadPrompt("Rfid", RfidOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.D:
                        CardDetected();
                        break;
                    case ConsoleKey.W:
                        WriteCard();
                        break;
                    case ConsoleKey.R:
                        ReadCard();
                        break;
                    case ConsoleKey.S:
                        ReadAllCardSectors(true);
                        break;
                    case ConsoleKey.M:
                        ReadAllCardSectors(false);
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
            while (!exit);
        }

        /// <summary>
        /// Tests the rfid controller.
        /// </summary>
        public static void TestRfidController()
        {
            Console.WriteLine("Testing RFID");
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;
                Console.WriteLine("Card detected");

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;

                // Print UID
                Console.WriteLine($"Card UID: {cardUid[0]},{cardUid[1]},{cardUid[2]},{cardUid[3]}");

                // Select the scanned tag
                device.SelectCardUniqueId(cardUid);

                // Writing data to sector 1 blocks
                // Authenticate sector
                if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, 7) == RFIDControllerMfrc522.Status.AllOk)
                {
                    var data = new byte[16 * 3];
                    for (var x = 0; x < data.Length; x++)
                    {
                        data[x] = (byte)(x + 65);
                    }

                    for (var b = 0; b < 3; b++)
                    {
                        device.CardWriteData((byte)(4 + b), data.Skip(b * 16).Take(16).ToArray());
                    }
                }

                // Reading data
                var continueReading = true;
                for (var s = 0; s < 16 && continueReading; s++)
                {
                    // Authenticate sector
                    if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, (byte)((4 * s) + 3)) == RFIDControllerMfrc522.Status.AllOk)
                    {
                        Console.WriteLine($"Sector {s}");
                        for (var b = 0; b < 3 && continueReading; b++)
                        {
                            var data = device.CardReadData((byte)((4 * s) + b));
                            if (data.Status != RFIDControllerMfrc522.Status.AllOk)
                            {
                                continueReading = false;
                                break;
                            }

                            Console.WriteLine($"  Block {b} ({data.Data.Length} bytes): {string.Join(" ", data.Data.Select(x => x.ToString("X2")))}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Authentication error");
                        break;
                    }
                }

                device.ClearCardSelection();
            }
        }

        private static void CardDetected()
        {
            Console.Clear();
            Console.WriteLine("Testing RFID");
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;
                Console.WriteLine("Card detected");

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;

                // Print UID
                Console.WriteLine($"Card UID: {cardUid[0]},{cardUid[1]},{cardUid[2]},{cardUid[3]}");

                Terminal.WriteLine(ExitMessage);

                while (true)
                {
                    var input = Console.ReadKey(true).Key;
                    if (input != ConsoleKey.Escape) continue;

                    break;
                }

                break;
            }
        }

        private static void WriteCard()
        {
            Terminal.Clear();
            Terminal.WriteLine("Testing RFID");

            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);
            var userInput = Terminal.ReadLine("Insert a message to be written in the card (16 characters only)").Truncate(16);
            Terminal.WriteLine("Place the card on the sensor");

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;

                // Select the scanned tag
                device.SelectCardUniqueId(cardUid);

                // Writing data to sector 1 blocks
                // Authenticate sector
                if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, 19) == RFIDControllerMfrc522.Status.AllOk)
                {
                    userInput = (userInput + new string(' ', 16)).Truncate(16);
                    device.CardWriteData(16, Encoding.ASCII.GetBytes(userInput));
                }

                device.ClearCardSelection();
                Terminal.WriteLine("Data has been written");

                Terminal.WriteLine(ExitMessage);

                while (true)
                {
                    var input = Console.ReadKey(true).Key;
                    if (input != ConsoleKey.Escape) continue;

                    break;
                }

                break;
            }
        }

        private static void ReadCard()
        {
            Console.Clear();
            Console.WriteLine("Testing RFID");
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;

                // Print UID
                Console.WriteLine($"Card UID: {cardUid[0]},{cardUid[1]},{cardUid[2]},{cardUid[3]}");

                // Select the scanned tag
                device.SelectCardUniqueId(cardUid);

                // Authenticate sector
                if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, 19) == RFIDControllerMfrc522.Status.AllOk)
                {
                    var data = device.CardReadData(16);
                    var text = Encoding.ASCII.GetString(data.Data);

                    Console.WriteLine($" Message read: {text}");
                }
                else
                {
                    Console.WriteLine("Authentication error");
                }

                device.ClearCardSelection();
                Terminal.WriteLine(ExitMessage);

                while (true)
                {
                    var input = Console.ReadKey(true).Key;
                    if (input != ConsoleKey.Escape) continue;

                    break;
                }

                break;
            }
        }

        private static void ReadAllCardSectors(bool readWithAuthentication)
        {
            Console.Clear();

            Console.WriteLine($"Testing RFID (Authentication={readWithAuthentication})");
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;

                // Print UID
                Console.WriteLine($"Card UID: {cardUid[0]},{cardUid[1]},{cardUid[2]},{cardUid[3]}");

                // Select the scanned tag
                device.SelectCardUniqueId(cardUid);

                // Reading data
                var continueReading = true;
                for (var s = 0; s < 16 && continueReading; s++)
                {
                    // Authenticate sector - not required for MIFARE Ultralight
                    if (!readWithAuthentication || device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, (byte)((4 * s) + 3)) == RFIDControllerMfrc522.Status.AllOk)
                    {
                        Console.WriteLine($"Sector {s}");
                        for (var b = 0; b < 3 && continueReading; b++)
                        {
                            var data = device.CardReadData((byte)((4 * s) + b));
                            if (data.Status != RFIDControllerMfrc522.Status.AllOk)
                            {
                                continueReading = false;
                                break;
                            }

                            Console.WriteLine($"  Block {b} ({data.Data.Length} bytes): {string.Join(" ", data.Data.Select(x => x.ToString("X2")))}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Authentication error, sector {s}");
                    }
                }

                device.ClearCardSelection();

                Terminal.WriteLine(ExitMessage);

                while (true)
                {
                    var input = Console.ReadKey(true).Key;
                    if (input != ConsoleKey.Escape) continue;

                    break;
                }

                break;
            }
        }
    }
}
