var jModel;
var koModel;
var stickyNotify;

function getModel() {
    koModel.startDate = $('#startDate').val();
    koModel.endDate = $('#endDate').val();
    $('#customerCodes tr').each(function (i, e) {
        koModel.customerCodes()[i].Checked = e.cells[0].children[0].checked;
    });
    $('#deskCodes tr').each(function (i, e) {
        koModel.deskCodes()[i].Checked = e.cells[0].children[0].checked;
    });
    $('#userCodes tr').each(function (i, e) {
        koModel.userCodes()[i].Checked = e.cells[0].children[0].checked;
    });

    koModel.selectedResultCodes = $('#preResultCodeSelect').val();
    koModel.selectedAgents = $('#preAgentSelect').val();
}

function reapply(data) {
    $('.table tbody').html("<tr><td><input data-bind='checked: Checked' type='checkbox' /></td><td><span data-bind='text: LabelValue'></span></td><td><span data-bind='text: CountValue'></span></td></tr>");

    koModel = {
        startDate: ko.observable(data.StartDate),
        endDate: ko.observable(data.EndDate),
        minLength: ko.observable(data.MinLength),
        maxLength: ko.observable(data.MaxLength),
        results: ko.observable(data.ResultCount),
        includeOnly: ko.observable(data.IncludeOnly),
        addToQueueId: ko.observable(data.AddToQueueId),
        customerCodes: ko.observableArray(data.CustomerCodes),
        deskCodes: ko.observableArray(data.DeskCodes),
        userCodes: ko.observableArray(data.UserCodes),
        allResultCodes: ko.observableArray(data.AllResultCodes),
        allAgents: ko.observableArray(data.AllAgents),
        selectedResultCodes: ko.observableArray(data.SelectedResultCodes),
        selectedAgents: ko.observableArray(data.SelectedAgents)
    }

    if (data.SelectedAgents.length || data.SelectedResultCodes.length) {
        $('#checkIncludeOnly').addClass('disabled');
        $('#checkIncludeOnly input').attr('disabled', 'disabled');
    }
    else {

        $('#checkIncludeOnly').removeClass('disabled');
        $('#checkIncludeOnly input').removeAttr('disabled');
    }

    ko.applyBindings(koModel);



    $("html").getNiceScroll().resize();
}

function postModel(e) {

    if (stickyNotify != null)
        $.gritter.remove(stickyNotify, {
            fade: true, // optional
            speed: 'fast' // optional
        });
    stickyNotify = $.gritter.add({
        title: 'Fetching Records',
        image: '/img/gritter-spinner.gif',
        text: 'Pulling info from the database...',
        sticky: true,
        class_name: 'gritter-light'
    });

    getModel();

    $.ajax({
        type: "POST",
        url: "/ManageQueue/Load",
        data: ko.toJSON({ model: koModel }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (stickyNotify != null)
                $.gritter.remove(stickyNotify, {
                    fade: true, // optional
                    speed: 'fast' // optional
                });
            $('#filterRow').show();
            $('#queueResults').show();
            $('#checkIncludeOnly').show();
            reapply(data);
            $.gritter.add({
                title: 'Records Returned',
                text: 'Returned ' + data.ResultCount + ' records...',
                sticky: false,
                class_name: 'gritter-light'
            });
            $('#getResults').text("Refresh Counts");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (stickyNotify != null)
                $.gritter.remove(stickyNotify, {
                    fade: true, // optional
                    speed: 'fast' // optional
                });
            $.gritter.add({
                title: textStatus,
                text: 'Error: ' + errorThrown,
                sticky: true
            });
        }
    });


    return false;
}

function addSelectionToQueue(e) {
    koModel.addToQueueId = "AD182134-5FF5-4507-9A98-4261BFA1AC06";
    return postModel(e);
}