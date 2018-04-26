﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainAPI.Tests
{
    class TestJsonObjectConsts
    {
        public const String Trader1ID = "TRADER1";
        public const String traderAuthentication = "{\"message\": \"Logged in as TRADER1\",\"access_token\": \"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpYXQiOjE1MjQ2OTMzMzQsIm5iZiI6MTUyNDY5MzMzNCwianRpIjoiYTkzYjZlYzEtNWM0Ny00YTVkLTlmMDQtYTUzMDcwYTRkNGQ3IiwiZXhwIjoxNTI0Njk0MjM0LCJpZGVudGl0eSI6IkdpYW5naCIsImZyZXNoIjpmYWxzZSwidHlwZSI6ImFjY2VzcyJ9.Xjtv0lO0sX_Hv1AH3xw6DWxlQFHSvOTbDqbqDT8CWko\",\"refresh_token\": \"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpYXQiOjE1MjQ2OTMzMzQsIm5iZiI6MTUyNDY5MzMzNCwianRpIjoiMjMwNjgwNzctMzZlNC00OWU5LWI5MjEtODFmMTJjYzM4OWEyIiwiZXhwIjoxNTI3Mjg1MzM0LCJpZGVudGl0eSI6IkdpYW5naCIsInR5cGUiOiJyZWZyZXNoIn0.6ddEx0tMZ3uyA4C-rM_J69teyri8I3cNLGAeneelBmI\"}";
        public const String messageFromFailLogin = "{\"message\": \"Wrong credentials\"}";
        public const String Trader1 = "{\"$class\": \"org.example.biznet.Trader\",\"traderId\": \"TRADER1\",\"firstName\": \"Alice\",\"lastName\": \"Margatroid\"}";
        public const String Trader2ID = "TRADER2";
        public const String Trader2 = "{\"$class\": \"org.example.biznet.Trader\",\"traderId\": \"TRADER2\",\"firstName\": \"Patchouli\",\"lastName\": \"Knowledge\"}";
        public const String listOfProperties = "[{\"$class\": \"org.example.biznet.Property\",\"PropertyId\": \"123\",\"description\": \"test\",\"owner\": \"resource:org.example.biznet.Trader#TRADER1\"},{\"$class\": \"org.example.biznet.Property\",\"PropertyId\": \"1234\",\"description\": \"test\",\"owner\": \"resource:org.example.biznet.Trader#TRADER1\"}]";
        public const String listOfTransactions = "[{\"$class\": \"org.example.biznet.Trade\",\"property\": \"resource:org.example.biznet.Property#Property%20A\",\"origOwner\": \"resource:org.example.biznet.Trader#TRADER1\",\"newOwner\": \"resource:org.example.biznet.Trader#TRADER3\",\"longitude\": -95.348388,\"latitude\": 29.72552,\"transactionId\": \"13b97fbb14db2dcd379385be681ee06958bba4a49b671e2bbc3efccf65c1521b\",\"timestamp\": \"2018-04-01T16:41:06.761Z\"},{\"$class\": \"org.example.biznet.Trade\",\"property\": \"resource:org.example.biznet.Property#Property%20A\",\"origOwner\": \"resource:org.example.biznet.Trader#TRADER2\",\"newOwner\": \"resource:org.example.biznet.Trader#TRADER1\",\"longitude\": -95.349048,\"latitude\": 29.722037,\"transactionId\": \"46673cd0df0c5ea42307defa4073aa443345e2eb40395f7bc2d11752aab5a240\",\"timestamp\": \"2018-04-01T16:41:04.269Z\"},{\"$class\": \"org.example.biznet.Trade\",\"property\": \"resource:org.example.biznet.Property#Property%20A\",\"origOwner\": \"resource:org.example.biznet.Trader#TRADER1\",\"newOwner\": \"resource:org.example.biznet.Trader#TRADER2\",\"longitude\": -95.342308,\"latitude\": 29.721115,\"transactionId\": \"7ad26a220481fd75a2ea61c5de7e10b6f85d96ef8416cc899d3c09db27df9362\",\"timestamp\": \"2018-04-01T16:41:01.791Z\"}]";
        public const String Trader1TransactionId = "13b97fbb14db2dcd379385be681ee06958bba4a49b671e2bbc3efccf65c1521b";
    }
}
