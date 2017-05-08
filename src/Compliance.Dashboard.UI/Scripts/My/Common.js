$(function () {
    $("#addCommentDialog").dialog({
        autoOpen: false,
        modal: true,
        resizeable: false,
        buttons: {
            Save: function () {
                var queueItemId = model.QueueItemId;
                var profileId = model.ProfileId;
                var baseUrl = '/api/service/QueueItemAction/';

                saveComment(queueItemId, profileId, baseUrl + queueItemId);

                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        },
        show: {
            effect: "drop",
            duration: 400,
            direction: "up"
        },
        hide: {
            effect: "drop",
            duration: 400,
            direction: "down"
        }
    });

    $("#pleaseWait").dialog({
        autoOpen: false,
        modal: true,
        resizeable: false,
        height: 120,
        show: {
            effect: "drop",
            duration: 400,
            direction: "up"
        },
        hide: {
            effect: "drop",
            duration: 400,
            direction: "down"
        }
    });

    $(".ui-dialog-titlebar").hide();

    $("#addComment").click(function () {
        $("#addCommentDialog").dialog("open");
        return false;
    });
});

function takeQueueAction(profileId, queueItemId, action, reason, postUrl, funcSuccess, funcFail) {
    var act = {};

    act.ItemId = queueItemId;
    act.AuditEntityType = "User";
    act.AuditEntityId = profileId;
    act.Comment = reason;
    act.ActionString = action;

    //Ajax post to the server
    $.ajax({
        url: postUrl,
        type: "POST",
        cache: false,
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(act),
        success: funcSuccess,
        error: funcFail
    })
};

function loader(status) {
    if (status == 'show') {
        $('#content').append('<div class="loading">Loading&#8230;</div>');
    }
    else {
        $('div.loading').remove();
    }
}

function showNotification(_message, _type) {

    switch (_type) {
        case 'success':
            toastr.success(_message);
            break;
        case 'error':
            toastr.error(_message)
            break;
        case 'warning':
            toastr.warning(_message)
            break;
        case 'info':
            toastr.info(_message)
            break;
    }
}

function showWait(theMessage) {
    $("#waitMsg").text(theMessage);
    $("#pleaseWait").dialog("open");
};

function hideWait() {
    $("#pleaseWait").dialog("close");
};

function stdError(errMsg) {
    hideWait();
    showWait("Error " + errMsg);
    setTimeout(hideWait, 2000);
};

function nextQueue(onTimout) {
    setTimeout(doNextQueue, 420);
};

function doNextQueue() {
    showWait("Getting next in queue...");
    if (model.Action == null) {
        window.location = "/PhoneAudit/queue/work/FirstQueue/1/";
    }
    else {
        window.location = "/PhoneAudit/queue/" + model.Action + "/" + model.Queue + "/" + model.Level + "/";
    }
};

function saveComment(queueItemId, profileId, postUrl) {

    var closeReason = $("#closeReason").val();

    if (closeReason != "None") {
        showWait("Closing Item...");

        takeQueueAction(
            profileId,
            queueItemId,
            "close",
            closeReason,
            postUrl,
            function () {
                hideWait();
                nextQueue();
            },
            function () {
                stdError("closing item!");
            });
    };
};

function postComment(profileId, workItemId, comment, postUrl, funcSuccess, funcFail) {

};

function askConfirm(title, msg, callback, arg) {
    $('<div></div>').appendTo('body')
                    .html('<div><h6>' + msg + '</h6></div>')
                    .dialog({
                        modal: true, title: title, zIndex: 10000, autoOpen: true,
                        width: 'auto', resizable: false,
                        buttons: {
                            Yes: function () {
                                callback(true, arg);
                                $(this).dialog("close");
                            },
                            No: function () {
                                callback(false, arg);
                                $(this).dialog("close");
                            }
                        },
                        close: function (event, ui) {
                            $(this).remove();
                        }
                    });
};