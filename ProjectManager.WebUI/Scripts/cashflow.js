ko.bindingHandlers.currency = {
    update: function(element, valueAccessor, allBindingsAccessor, viewModel) {
        var amount = Number(ko.utils.unwrapObservable(valueAccessor()) || 0);

        var converted = '$' + amount.toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");

        return ko.bindingHandlers.text.update(element, function () {
            return converted;
        });
    }
};

ko.bindingHandlers.date = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var date = ko.utils.unwrapObservable(valueAccessor());

        var converted = new Date(date);

        converted.setDate(converted.getDate() + 1);

        var dateStr = converted.getMonth() + 1 + "/" + converted.getDate() + "/" + converted.getFullYear();

        return ko.bindingHandlers.text.update(element, function () {
            return dateStr;
        });
    }
};

var viewModel = function () {
    var self = this;

    self.transactions = ko.observableArray([]);
    self.beginningBalance = ko.observable();
}

var appModel = new viewModel();

$(document).ready(function () {
    getData();
});

function getData() {
    var url = window.location.origin + "/api/cashflow";

    $.ajax({
        url: url,
        type: 'GET',
        contentType: "application/json",
        datatype: "json",
        success: function (data) {
            appModel.transactions(data.XeroTransactions);
            appModel.beginningBalance(data.Balance.toFixed(2));
            ko.applyBindings(appModel);
            calculateBalances();
            addListeners();
            removeLoadingScreen();
        },
        error: function (err) {
            console.log('error : ' + err.message);
        }
    });
}

function calculateBalances() {
    var lastBalanceCalculation = Number(appModel.beginningBalance());

    var transactionIndexes = $('.transaction input[name=ignore]:checkbox:not(:checked)')
        .map(function (index, item) {
            var pos = item.id.indexOf('_') + 1;
            return item.id.substring(pos);
        }).toArray();

    var transactions = appModel.transactions();

    transactionIndexes.forEach(function (item, index, arr) {
        lastBalanceCalculation = lastBalanceCalculation + transactions[item].Amount;

        var balance = Number(lastBalanceCalculation);

        var converted = '$' + balance.toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");

        $('#balance_' + item).html(converted);
    });
}

function addListeners()
{
    $('table').on('click', 'input[name=ignore]', function () {
        var pos = this.id.indexOf('_') + 1;
        var id = this.id.substring(pos);

        var balance = $('#balance_' + id);
        balance.html('...');

        var amount = balance.siblings('.amount');
        amount.toggleClass('strike');

        calculateBalances();
    });
}

function removeLoadingScreen() {
    $(".loading").fadeOut("slow");
}
