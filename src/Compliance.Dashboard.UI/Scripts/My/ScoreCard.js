
var scoreCard = {
    saved: false,
    refSaved: false,
    urlItemActionBase: '/api/service/QueueItemAction/',
    urlSaveScoreCardBase: '/api/service/SaveAnswers/',
    urlAddWorkerRelationBase: '/api/service/AddWorker/'
};

function answer_select(aId) {
    //Get the assignment if we have never got one
    var theDate = new Date();
    var queueItemId = model.QueueItemId;
    var profileId = model.ProfileId;
    var postUrl = scoreCard.urlItemActionBase  + queueItemId
    model.LastAssignment;

    if (model.LastAssignment == "") {
        model.LastAssignment == theDate.toUTCString();
        takeQueueAction(
            profileId,
            queueItemId,
            "assign",
            "",
            postUrl,
            null,
            function () {
                stdError("assigning item to you.");
            });
    };


    //Handle the answer click
    var $this = $("#" + aId);
    var qId = $this.attr('data-question');
    var childQuestions = 'parent_' + aId;
    var allAnswers = $("[data-question='" + qId + "']")
    var myLabel = $('#lbl_' + qId);

    allAnswers.each(function (index) {
        $('#lbl_' + $(this).attr('data-question')).text('N/A')
        $('.parent_' + $(this).attr('id')).each(function (i) {
            $('#lbl_' + $(this).attr('id')).text('N/A');
            $(this).hide("slow");
        });
    });

    myLabel.text($this.text());
    myLabel.attr({
        "data-answer": aId,
        "data-comment": $(".jp-current-time").first().text()
    });

    $('.' + childQuestions).each(function (index) {
        $(this).show("slow");
        $('#lbl_' + $(this).attr('id')).text('');
    });

    var incomplete = false;
    $(".answer_" + myLabel.attr('data-scorecard')).each(function (i) {
        if ($(this).text() == "")
            incomplete = true;
    });

    if (!incomplete)
    {
        if ($(".chkUserRef:checked").length == 0)
        {
            askConfirm("Select corresponding note", "You have not selected a note that corresponds to the call.<br /><br />Continue saving score card?", function (ans, arg) {
                if (ans == true)
                    saveScoreCard(arg);
            }, myLabel.attr('data-scorecard'));

            return;
        }

        saveScoreCard(myLabel.attr('data-scorecard'));
    };
}

function saveScoreCard(scId) {
      
    //Disable form
    showWait("Saving ScoreCard...");

    //Build ScoreCardResult
    var sc = {};

    sc.MyScoreCardId = scId;
    sc.WorkItemType = "Recording";
    sc.WorkItemId = model.WorkItemId;
    sc.AuditEntityType = "User";
    sc.AuditEntityId = model.ProfileId;

    //Get answers
    var answers = [];
    $(".answer_" + scId).each(function (i) {
        var i = $(this);
        if (i.text() != "N/A") {
            answers.push({
                MyAnswerId: i.attr("data-answer"),
                Comment: i.attr("data-comment")
            });
        }
    });

    sc.Answers = answers;

    //Add answers and result to the cmd
    var sscrCmd = { MyQueueItemId: model.QueueItemId, MyScoreCardResult: sc };

    scoreCard.saved = false;
    scoreCard.refSaved = false;

    //Ajax post to the server
    $.ajax({
        url: scoreCard.urlSaveScoreCardBase + scId,
        type: "POST",
        cache: false,
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(sscrCmd),
        success: function () {
            markSaved();
        },
        error: function () {
            stdError("saving scorecard results!");
        }
    })

    var refs = [];

    $(".chkUserRef:checked").each(function (i) {
        var i = $(this);
        refs.push({
            Identifier: i.attr("data-user"),
            AuditEntityId: model.ProfileId,
            AuditEntityType: "User"
        });
    });

    var refCmd = { Relationships: refs };

    //Ajax post to the server
    $.ajax({
        url: scoreCard.urlAddWorkerRelationBase + model.WorkItemId,
        type: "POST",
        cache: false,
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(refCmd),
        success: function () {
            markRefSaved();
        },
        error: function () {
            stdError("saving user reference!");
        }
    })
}

function markSaved() {

    scoreCard.saved = true;

    if (scoreCard.refSaved) {
        //Set call back to next in queue
        hideWait();
        nextQueue();
    }
}

function markRefSaved() {

    scoreCard.refSaved = true;

    if (scoreCard.saved) {
        //Set call back to next in queue
        hideWait();
        nextQueue();
    }
}
