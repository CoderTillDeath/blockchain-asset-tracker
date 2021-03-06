﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlockchainAPI.Models;
using Moq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using BlockchainAPI.Transactions;
using System.Linq;

namespace BlockchainAPI.Tests
{
    [TestClass]
    public class BlockchainClientTests
    {
        private BlockchainClient clientWithMock;
        Mock<IBlockchainService> mockBlockService;
        User user;

        [TestInitialize()]
        public void init()
        {
            mockBlockService = new Mock<IBlockchainService>();
            clientWithMock = new BlockchainClient(mockBlockService.Object);
            user = new User() { username = "TRADER1" };
        }

        [TestMethod]
        public void CanaryTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task Login_Method_for_client_should_set_username()
        {
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.TraderQueryURL(TestJsonObjectConsts.trader1ID)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1);
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(user)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1AuthenticationMessage);

            await clientWithMock.Login(user);

            Assert.AreEqual(TestJsonObjectConsts.trader1ID, clientWithMock.thisTrader.traderId);
        }

        [TestMethod]
        public async Task Login_Method_will_call_invokeget_to_check_for_users_existance()
        {
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(user)))
                            .ReturnsAsync("{}");

            await clientWithMock.Login(user);

            mockBlockService.Verify(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(user)));
        }


        [TestMethod]
        public async Task If_username_does_not_exist_then_login_return_false()
        {
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(new User() { username = "notExist" })))
                            .ReturnsAsync(TestJsonObjectConsts.messageFromFailLogin);

            bool loginSucess = await clientWithMock.Login(new User() { username = "notExist" });

            Assert.IsFalse(loginSucess);
        }

        [TestMethod]
        public async Task Failed_logins_will_also_not_set_trader_object()
        {
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(new User() { username = "notExist" })))
                            .ReturnsAsync(TestJsonObjectConsts.messageFromFailLogin);

            bool loginSuccess = await clientWithMock.Login(new User() { username = "notExist" });

            Assert.IsNull(clientWithMock.thisTrader);
        }

        [TestMethod]
        public async Task NetworkFailWhenLogin()
        {
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(new User() { username = "exist", password = "password", firstName = "f", lastName = "l" })))
                            .ThrowsAsync(new HttpRequestException());

            bool result = await clientWithMock.Login(new User() { username = "exist", password="password", firstName = "f", lastName = "l" });

            Assert.AreEqual(false, result);
        }

        public void AssertTradersEqual(Trader t1, Trader t2)
        {
            Assert.AreEqual(t1.firstName, t2.firstName);
            Assert.AreEqual(t1.lastName, t2.lastName);
            Assert.AreEqual(t1.objectType, t2.objectType);
            Assert.AreEqual(t1.traderId, t2.traderId);
        }

        [TestMethod]
        public async Task Successful_login_will_set_trader_object_to_serialized_json()
        {
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.TraderQueryURL(TestJsonObjectConsts.trader1ID)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1);
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(user)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1AuthenticationMessage);
            Trader expectedTrader = JsonConvert.DeserializeObject<Trader>(TestJsonObjectConsts.trader1);

            bool results = await clientWithMock.Login(user);

            AssertTradersEqual(expectedTrader, clientWithMock.thisTrader);
        }

        [TestMethod]
        public async Task Get_My_Assets_Will_InvokeGet_From_BlockchainService()
        {
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.TraderQueryURL(TestJsonObjectConsts.trader1ID)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1);
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(user)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1AuthenticationMessage);
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.MyAssetsUrl(TestJsonObjectConsts.trader1ID)))
                            .ReturnsAsync("[]");

            await clientWithMock.Login(user);
            List<Property> results = await clientWithMock.GetMyProperties();

            mockBlockService.Verify(m => m.InvokeGet(HyperledgerConsts.TraderQueryURL(TestJsonObjectConsts.trader1ID)));
        }

        [TestMethod]
        public void Trader_object_has_extra_fullname_field_for_convenience()
        {
            Trader t = new Trader();

            t.firstName = "Hello";
            t.lastName = "World";
            string expectResult = String.Format("{0} {1}", t.firstName, t.lastName);

            Assert.AreEqual(expectResult, t.fullName);
        }

        [TestMethod]
        public async Task RegisterNewTrader_will_invoke_post()
        {
            await clientWithMock.RegisterNewTrader(user);

            mockBlockService.Verify(m => m.InvokePostAuthentication(FlaskConsts.RegistrationUrl, JsonConvert.SerializeObject(user)));
        }

        [TestMethod]
        public async Task RegisterNewProperty_will_invoke_post()
        {
            Property property = new Property();
            property.PropertyId = "123";
            property.description = "456";
            
            await clientWithMock.RegisterNewProperty(property);

            mockBlockService.Verify(m => m.InvokePost(HyperledgerConsts.PropertyUrl, JsonConvert.SerializeObject(property)));
        }

        [TestMethod]
        public async Task sendProperty_will_invoke_post()
        {
            mockBlockService.Setup(m => m.InvokeHead(HyperledgerConsts.TraderUrl, TestJsonObjectConsts.trader1ID))
                            .ReturnsAsync(true);
            Transaction transaction = new Transaction();
            transaction.newOwner = TestJsonObjectConsts.trader1ID;


            await clientWithMock.SendProperty(transaction);

            mockBlockService.Verify(m => m.InvokePost(HyperledgerConsts.TransactionUrl, JsonConvert.SerializeObject(transaction)));
        }

        [TestMethod]
        public async Task If_network_is_down_sendProperty_return_false()
        {
            mockBlockService.Setup(m => m.InvokeHead(HyperledgerConsts.TraderUrl, TestJsonObjectConsts.trader1ID))
                            .ReturnsAsync(true);
            mockBlockService.Setup(m => m.InvokePost(It.IsAny<String>(), It.IsAny<String>()))
                            .ThrowsAsync(new HttpRequestException());

            BlockchainClient.Result error = await clientWithMock.SendProperty(new Transaction() {newOwner = TestJsonObjectConsts.trader1ID });

            Assert.AreEqual(BlockchainClient.Result.NETWORK, error);
        }
        
        [TestMethod]
        public async Task send_property_return_false_when_user_already_exist()
        {
            mockBlockService.Setup(m => m.InvokeHead(It.IsAny<String>(),It.IsAny<String>()))
                            .ReturnsAsync(false);

            BlockchainClient.Result error = await clientWithMock.SendProperty(new Transaction());

            Assert.AreEqual(BlockchainClient.Result.EXISTERROR, error);
        }
        
        [TestMethod]
        public void JsonConvertCanDeserializeEvenWhenThereAreExtraFields()
        {
            Trader trader = JsonConvert.DeserializeObject<Trader>(TestJsonObjectConsts.trader1);
            string expectedResult = String.Format("{0} {1}", trader.firstName, trader.lastName);

            Assert.AreEqual(expectedResult, trader.fullName);
        }

        [TestMethod]
        public async Task If_network_is_down_cant_register_properties()
        {
            Property property = new Property();
            property.PropertyId = "123";
            property.description = "456";
            mockBlockService.Setup(m => m.InvokePost(HyperledgerConsts.PropertyUrl, It.IsAny<String>()))
                            .ThrowsAsync(new HttpRequestException());

            BlockchainClient.Result error = await clientWithMock.RegisterNewProperty(property);

            Assert.AreEqual(BlockchainClient.Result.NETWORK, error);
        }

        [TestMethod]
        public async Task If_PropertyId_and_description_is_null_return_Empty()
        {
            Property property = new Property();

            mockBlockService.Setup(m => m.InvokePost(HyperledgerConsts.PropertyUrl, It.IsAny<String>()))
                            .ThrowsAsync(new HttpRequestException());

            BlockchainClient.Result error = await clientWithMock.RegisterNewProperty(property);

            Assert.AreEqual(BlockchainClient.Result.EMPTY, error);
        }

        [TestMethod]
        public async Task cant_register_new_property_if_it_exist()
        {
            Property property = new Property();
            property.PropertyId = "123";
            property.description = "456";
            mockBlockService.Setup(m => m.InvokeHead(It.IsAny<String>(), It.IsAny<String>()))
                            .ReturnsAsync(true);

            BlockchainClient.Result error = await clientWithMock.RegisterNewProperty(property);

            Assert.AreEqual(BlockchainClient.Result.EXISTERROR, error);
        }

        [TestMethod]
        public async Task If_network_is_down_cant_register_trader()
        {
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.RegistrationUrl, It.IsAny<String>()))
                            .ThrowsAsync(new HttpRequestException());

            BlockchainClient.Result error = await clientWithMock.RegisterNewTrader(new User());

            Assert.AreEqual(error, BlockchainClient.Result.NETWORK);
        }

        [TestMethod]
        public async Task If_network_is_down_cant_get_property()
        {
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.TraderQueryURL(TestJsonObjectConsts.trader1ID)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1);
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(user)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1AuthenticationMessage);
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.MyAssetsUrl(TestJsonObjectConsts.trader1ID)))
                            .ThrowsAsync(new HttpRequestException());

            await clientWithMock.Login(user);
            List<Property> results = await clientWithMock.GetMyProperties();

            Assert.IsNull(results);
        }

        [TestMethod]
        public async Task GetUserTransactionsWillInvokeCreatePackageUrl()
        {
            mockBlockService.Setup(m => m.InvokeGet(It.IsAny<String>()))
                            .ReturnsAsync("[]");


            var result = await clientWithMock.GetUserTransactions();

            mockBlockService.Verify(m => m.InvokeGet(It.IsAny<String>()));
        }

        [TestMethod]
        public async Task GetAllTransactionsWillInvokeGet()
        {
            mockBlockService.Setup(m => m.InvokeGet(It.IsAny<String>()))
                            .ReturnsAsync("[]");

            await clientWithMock.GetAllTransactions();

            mockBlockService.Verify(m => m.InvokeGet(HyperledgerConsts.OrderedTransactionUrl));
        }

        [TestMethod]
        public async Task PropertyHistoryWillInvokeANewURL()
        {
            mockBlockService.Setup(m => m.InvokeGet(It.IsAny<String>()))
                            .ReturnsAsync("[]");

            await clientWithMock.GetPropertyHistory("Property A");

            mockBlockService.Verify(m => m.InvokeGet(HyperledgerConsts.PropertyPackageUrl("Property%20A")));
        }

        [TestMethod]
        public async Task PropertyHistoryWillCallPackageHistoryOnEveryReturnedPackage()
        {
            string property = "Property A";
            string packageList = "[{\"PackageId\": \"PackageA\"}, {\"PackageId\": \"PackageB\"}]";
            List<Package> actualList = JsonConvert.DeserializeObject<List<Package>>(packageList);
            mockBlockService.Setup(m => m.InvokeGet(It.IsAny<String>()))
                            .ReturnsAsync("[]");
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.PropertyPackageUrl(Uri.EscapeDataString(property))))
                            .ReturnsAsync(packageList);

            await clientWithMock.GetPropertyHistory(property);

            foreach(Package package in actualList)
            {
                mockBlockService.Verify(m => m.InvokeGet(HyperledgerConsts.PackageHistoryUrl(package.PackageId)));
            }
        }

        [TestMethod]
        public async Task CreatePackageShouldInvokeTheRightURL()
        {
            CreatePackage package = new CreatePackage();

            package.packageId = "testID";
            package.sender = "sender";
            package.recipient = "recipient";
           
            mockBlockService.Setup(m => m.InvokeHead(HyperledgerConsts.TraderUrl, It.IsAny<String>()))
                            .ReturnsAsync(true);
            await clientWithMock.CreatePackage(package);

            mockBlockService.Verify(m => m.InvokePost(HyperledgerConsts.CreatePackageUrl, It.IsAny<String>()));
        }

        [TestMethod]
        public async Task CreatePackagesEnterUnExistingReceipient()
        {
            CreatePackage package = new CreatePackage()
            {
                packageId = "testId",
                sender = "sender",
                recipient = "recipient"
            };

            mockBlockService.Setup(m => m.InvokeHead(It.IsAny<String>(), It.IsAny<String>()))
                            .ReturnsAsync(false);

            BlockchainClient.Result notExist = await clientWithMock.CreatePackage(package);

            Assert.AreEqual(notExist, BlockchainClient.Result.EXISTERROR);
        }

        [TestMethod]
        public async Task CreatePackageNetworkDown()
        {
            CreatePackage package = new CreatePackage()
            {
                packageId = "testId",
                sender = "sender",
                recipient = "recipient"
            };

            mockBlockService.Setup(m => m.InvokeHead(HyperledgerConsts.TraderUrl, It.IsAny<String>()))
                            .ReturnsAsync(true);

            mockBlockService.Setup(m => m.InvokePost(HyperledgerConsts.CreatePackageUrl, It.IsAny<String>()))
                            .ThrowsAsync(new HttpRequestException());


            BlockchainClient.Result networkError = await clientWithMock.CreatePackage(package);

            Assert.AreEqual(BlockchainClient.Result.NETWORK, networkError);
        }

        [TestMethod]
        public async Task UnboxPackageShouldInvokeTheRightURL()
        {
            UnboxPackage package = new UnboxPackage();

            await clientWithMock.UnboxPackage(package);

            mockBlockService.Verify(m => m.InvokePost(HyperledgerConsts.UnboxPackageUrl, JsonConvert.SerializeObject(package)));
        }

        [TestMethod]
        public async Task UserExistsShouldInsteadUseHead()
        {
            string username = It.IsAny<String>();

            await clientWithMock.UserExists(username);

            mockBlockService.Verify(m => m.InvokeHead(HyperledgerConsts.TraderUrl, username));
        }

        [TestMethod]
        public async Task userExistReturnFailWhenServiceIsDown()
        {
            mockBlockService.Setup(m => m.InvokeHead(HyperledgerConsts.TraderUrl, TestJsonObjectConsts.trader1ID))
                            .ThrowsAsync(new HttpRequestException());

            bool isUserExist = await clientWithMock.UserExists(TestJsonObjectConsts.trader1ID);

            Assert.IsFalse(isUserExist);
        }

        [TestMethod]
        public async Task PropertyExistsReturnFailWhenServiceIsDown()
        {
            string propertyId = It.IsAny<String>();
            mockBlockService.Setup(m => m.InvokeHead(HyperledgerConsts.PropertyUrl, propertyId))
                            .ThrowsAsync(new HttpRequestException());

            bool isPropertyExist = await clientWithMock.PropertyExists(propertyId);

            Assert.IsFalse(isPropertyExist);
        }

        [TestMethod]
        public async Task PropertyExistsShouldInsteadUseHead()
        {
            string propertyId = It.IsAny<String>();

            await clientWithMock.PropertyExists(propertyId);

            mockBlockService.Verify(m => m.InvokeHead(HyperledgerConsts.PropertyUrl, propertyId));
        }

        [TestMethod]
        public void TestSortByDate()
        {
            List<Transfer> list = new List<Transfer>
            {
                new Transfer {timestamp = DateTime.Today},
                new Transfer {timestamp = DateTime.Today.AddDays(-1)},
                new Transfer {timestamp = DateTime.Today.AddDays(1)}
            };

            list = list.OrderBy(x => x.timestamp).ToList();

            for(int i = 0; i < list.Count()-1; i++)
            {
                Assert.IsTrue(list[i].timestamp < list[i+1].timestamp);
            }
        }

        [TestMethod]
        public async Task PropertyHistoryWillReturnListOfTransfersInOrderWithMostRecentFirst()
        {
            string property = "Property A";
            string package = "PackageA";
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.PackageHistoryUrl(Uri.EscapeDataString(package))))
                            .ReturnsAsync("[{\"timestamp\":\"2018-04-21T19:47:54.368Z\"},{\"timestamp\":\"2018-04-21T19:48:54.368Z\"}]");
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.PropertyPackageUrl(Uri.EscapeDataString(property))))
                            .ReturnsAsync("[{\"PackageId\": \"PackageA\"}]");

            List<NewTransfer> results = await clientWithMock.GetPropertyHistory(property);

            for (int i = 0; i < results.Count() - 1; i++)
            {
                Assert.IsTrue(results[i].timestamp > results[i + 1].timestamp);
            }
        }

        [TestMethod]
        public async Task TestPropertyHistoryOnMultiplePackages()
        {
            string property = "Property A";
            string package1 = "PackageA";
            string package2 = "PackageB";
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.PackageHistoryUrl(Uri.EscapeDataString(package1))))
                            .ReturnsAsync("[{\"timestamp\":\"2018-04-21T19:47:54.368Z\"},{\"timestamp\":\"2018-04-21T19:48:54.368Z\"}]");
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.PackageHistoryUrl(Uri.EscapeDataString(package2))))
                            .ReturnsAsync("[{\"timestamp\":\"2018-04-21T19:46:54.368Z\"},{\"timestamp\":\"2018-04-21T19:48:58.368Z\"}]");
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.PropertyPackageUrl(Uri.EscapeDataString(property))))
                            .ReturnsAsync("[{\"PackageId\": \"PackageA\"},{\"PackageId\": \"PackageB\"}]");

            List<NewTransfer> results = await clientWithMock.GetPropertyHistory(property);

            for (int i = 0; i < results.Count() - 1; i++)
            {
                Assert.IsTrue(results[i].timestamp > results[i + 1].timestamp);
            }
        }

        [TestMethod]
        public async Task AddNewTransferMethodWillInvokePoseNewTransferUrl()
        {
            mockBlockService.Setup(m => m.InvokePost(HyperledgerConsts.NewTransferUrl, It.IsAny<String>()))
                           .ReturnsAsync("[]");
            NewTransfer transfer = new NewTransfer();

            await clientWithMock.AddNewTransfer(transfer);

            mockBlockService.Verify(m => m.InvokePost(HyperledgerConsts.NewTransferUrl, It.IsAny<String>()));
        }

        [TestMethod]
        public async Task addNewTrasferReturnNetWorkErrorWhenNetWorkIsDown()
        {
            mockBlockService.Setup(m => m.InvokePost(HyperledgerConsts.NewTransferUrl, It.IsAny<String>()))
                           .ThrowsAsync(new HttpRequestException());
            NewTransfer transfer = new NewTransfer();

            var error = await clientWithMock.AddNewTransfer(transfer);

            Assert.AreEqual(BlockchainClient.Result.NETWORK, error);
        }

        [TestMethod]
        public async Task GetAllTransfersWillBeInReverseOrder()
        {
            mockBlockService.Setup(m => m.InvokeGet(It.IsAny<String>()))
                            .ReturnsAsync("[{\"timestamp\":\"2018-04-21T19:47:54.368Z\"},{\"timestamp\":\"2018-04-21T19:46:54.368Z\"}]");

            List<NewTransfer> results = await clientWithMock.GetAllTransfers();

            for (int i = 0; i < results.Count() - 1; i++)
            {
                Assert.IsTrue(results[i].timestamp > results[i + 1].timestamp);
            }
        }

        [TestMethod]
        public void GetBlockchainService()
        {
            IBlockchainService service = clientWithMock.GetBlockChainService();

            Assert.IsNotNull(service);

        }
        
        [TestMethod]
        public async Task GetMyPackagesMethodWillInvokeMyPackagesUrl()
        {
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.MyPackagesUrl(TestJsonObjectConsts.trader1ID)))
                            .ReturnsAsync("[]");
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.TraderQueryURL(TestJsonObjectConsts.trader1ID)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1);
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(user)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1AuthenticationMessage);

            await clientWithMock.Login(user);

            List<Package> results = await clientWithMock.GetMyPackages();

            mockBlockService.Verify(m => m.InvokeGet(HyperledgerConsts.MyPackagesUrl(TestJsonObjectConsts.trader1ID)));
        }

        [TestMethod]
        public async Task CantGetPackageWhenServiceIsDown()
        {
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.MyPackagesUrl(TestJsonObjectConsts.trader1ID)))
                            .ThrowsAsync(new HttpRequestException());
            mockBlockService.Setup(m => m.InvokeGet(HyperledgerConsts.TraderQueryURL(TestJsonObjectConsts.trader1ID)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1);
            mockBlockService.Setup(m => m.InvokePostAuthentication(FlaskConsts.LoginUrl, JsonConvert.SerializeObject(user)))
                            .ReturnsAsync(TestJsonObjectConsts.trader1AuthenticationMessage);

            await clientWithMock.Login(user);

            List<Package> results = await clientWithMock.GetMyPackages();

            Assert.IsNull(results);
        }

        [TestMethod]
        public void DataIsNotNullWhenSetSelectedData()
        {
            SelectedData<Property> selectedData = new SelectedData<Property>();

            selectedData.data = new Property();
            selectedData.selected = true;

            Assert.IsNotNull(selectedData);
        }

        [TestMethod]
        public async Task testSendQrCodeWillNotThrowAnyException()
        {
            await clientWithMock.SendQRCode("email@gmail.com", "propertyId");
            Assert.IsTrue(true);
        }
    }
}
