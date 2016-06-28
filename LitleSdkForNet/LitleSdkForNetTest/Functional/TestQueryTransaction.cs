﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Litle.Sdk;

namespace Litle.Sdk.Test.Functional
{
    [TestFixture]
    class TestQueryTransaction
    {
        private LitleOnline litle;
        private Dictionary<string, string> config;

        [TestFixtureSetUp]
        public void SetUpLitle()
        {
            config = new Dictionary<string, string>();
            config.Add("url", "https://www.testlitle.com/sandbox/communicator/online");
            config.Add("reportGroup", "Default Report Group");
            config.Add("username", "DOTNET");
            config.Add("version", "8.13");
            config.Add("timeout", "65");
            config.Add("merchantId", "101");
            config.Add("password", "TESTCASE");
            config.Add("printxml", "true");
            config.Add("proxyHost", Properties.Settings.Default.proxyHost);
            config.Add("proxyPort", Properties.Settings.Default.proxyPort);
            config.Add("logFile", Properties.Settings.Default.logFile);
            config.Add("neuterAccountNums", "true");
            litle = new LitleOnline(config);
        }

        [Test]
        public void SimpleQueryTransaction()
        {
            queryTransaction query = new queryTransaction();
            query.id = "myId";
            query.origId = "Deposit1";
            query.origActionType = actionTypeEnum.D;
            query.origLitleTxnId = 54321;
            Console.WriteLine("Report Group: " + query.reportGroup);


            transactionTypeWithReportGroup response = litle.queryTransaction(query);
            queryTransactionResponse queryResponse = (queryTransactionResponse)response;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("000", queryResponse.response);
            Assert.AreEqual("Original transaction found", queryResponse.message);
            Assert.AreEqual("000", ((captureResponse)queryResponse.results_max10[0]).response);

        }

        [Test]
        public void SimpleQueryTransaction_MultipleResponses()
        {
            queryTransaction query = new queryTransaction();
            query.id = "myId";
            query.reportGroup = "myReportGroup";
            query.origId = "Auth2";
            query.origActionType = actionTypeEnum.A;
            query.origLitleTxnId = 54321;


            transactionTypeWithReportGroup response = litle.queryTransaction(query);
            queryTransactionResponse queryResponse = (queryTransactionResponse)response;

            Assert.NotNull(queryResponse);
            Assert.AreEqual("000", queryResponse.response);
            Assert.AreEqual("Original transaction found", queryResponse.message);
            Assert.AreEqual(2, queryResponse.results_max10.Count);

            Assert.AreEqual("000", ((authorizationResponse)queryResponse.results_max10[0]).response);
            Assert.AreEqual("Approved", ((authorizationResponse)queryResponse.results_max10[0]).message);

            Assert.AreEqual("000", ((authorizationResponse)queryResponse.results_max10[1]).response);
            Assert.AreEqual("Approved", ((authorizationResponse)queryResponse.results_max10[1]).message);

        }

        [Test]
        public void testQueryTransactionUnavailableResponse()
        {
            queryTransaction query = new queryTransaction();
            query.id = "myId";
            query.reportGroup = "myReportGroup";
            query.origId = "Auth";
            query.origActionType = actionTypeEnum.A;
            query.origLitleTxnId = 54321;


            transactionTypeWithReportGroup response = litle.queryTransaction(query);
            queryTransactionUnavailableResponse queryResponse = (queryTransactionUnavailableResponse)response;

            Assert.AreEqual("152", queryResponse.response);
            Assert.AreEqual("Original transaction found but response not yet available", queryResponse.message);
        }

        [Test]
        public void testQueryTransactionNotFoundResponse()
        {
            queryTransaction query = new queryTransaction();
            query.id = "myId";
            query.reportGroup = "myReportGroup";
            query.origId = "Auth0";
            query.origActionType = actionTypeEnum.A;
            query.origLitleTxnId = 54321;


            transactionTypeWithReportGroup response = litle.queryTransaction(query);
            queryTransactionResponse queryResponse = (queryTransactionResponse)response;

            Assert.AreEqual("151", queryResponse.response);
            Assert.AreEqual("Original transaction not found", queryResponse.message);
        }
    }
}
