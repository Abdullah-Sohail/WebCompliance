var currentPage = 1;
var sortAsc = true;
var timer;

function init(data) {
    if (data) {
        furnishDatatable(data);
    }

    $(".searchTeams").keydown(function () {
        clearTimeout(timer);  //clear any running timeout on key up
        timer = setTimeout(function () { //then give it a second to see if the user is finished
            getTeams();
        }, 500);
    });


    $("#page").click(function () {
        return false;
    });

    $('body').on('click', '.createTeam', function () {
        createTeam(this);
    });

    $('body').on('click', '.editTeam', function () {
        editTeam(this);
    });

    $('body').on('click', '.deleteTeam', function () {
        deleteTeam(this);
    });
    
    $('body').on('click', '.assignQueue', function () {
        manageQueue(this);
    });

    $('body').on('click', '.saveQueue', function () {
        updateQueueList();
    });
}

function updateQueueList() {
    var element = $(".select2");
    var id = $("#teamHidden").val();
    var queues = $(".select2").select2("val");
    var ids = [];
    for (var i = 0; i < queues.length; i++){
        ids.push(queues[i]);
    }
    var myurl = "/Team/UpdateQueue";
    var _data = {
        Id: id,
        SelectedQueues:ids
    }
    $.ajax({
        type: "POST",
        url: myurl,
        data:_data,
        dataType: "json",
        success: function (response) {
            loader('hide');
            if (response.IsSuccess) {
                $('#modalQueue').modal('hide');
                console.log("Queue Updated!", "success");
            }
            else {
                console.log("Error!", response.msg, "error");
            }
         },
        failure: function (response) {
            loader('hide');
            console.log(response);
        },
        error: function (response) {
            loader('hide');
            console.log(response);
        }
    });

}
function manageQueue(element) {
    var id = $(element).attr("data-id");
    $("#teamHidden").val(id);
    var url = "/Team/AllQueue";
    loader('show');
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (response) {
            loader('hide');

            var queue = response;
            $.map(queue, function (val, i) {
                val.id = val.Id,
                val.text = val.QueueName
            });
            $('#modalQueue').modal('show');
            $('.select2').select2({
                data: queue,
                multiple:true

            });
        },
        failure: function (response) {
            loader('hide');
            console.log(response);
        },
        error: function (response) {
            loader('hide');
            console.log(response);
        }
    });

   
}

function sortTeams(field, column) {
    $("#team-data-table thead th span").attr('class', 'pull-right icon-sort');
    var icon = !sortAsc ? "icon-sort-up" : "icon-sort-down";
    $(column).find("span").removeClass("icon-sort").addClass(icon);
    sortAsc = !sortAsc;
    getTeams(currentPage);
}

function loader(status) {
    if (status == 'show') {
        $('#content').append('<div class="loading">Loading&#8230;</div>');
    }
    else {
        $('div.loading').remove();
    }
}

function getTeams(page) {
    if (!page) {
        page = 1;
    }


    var search = $(".searchTeams").val();
    var url = "/Team/Index?page=" + page + "&search=" + search + "&sortField=Name&isAsc=" + sortAsc

    if (search == "")
        loader('show');

    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (response) {
            loader('hide');
            furnishDatatable(response)
        },
        failure: function (response) {
            loader('hide');
            console.log(response);
        },
        error: function (response) {
            loader('hide');
            console.log(response);
        }
    });
}

function furnishDatatable(data) {
    currentPage = data.Pager.CurrentPage;
    $("#team-data-table tbody").empty();

    $('#teamRowTmpl')
        .tmpl(data.Items).appendTo('#team-data-table tbody');

    $('#pagination').empty();
    $('#paginationTmpl')
        .tmpl(data).appendTo('#pagination');
}

function nTimesArray(n) {
    var arr = [];
    for (var i = 01; i <= n; i++) {
        arr.push(i);
    }
    return arr;
}

function createTeam() {

    swal({
        title: "Create new team",
        text: "",
        type: "input",
        showCancelButton: true,
        closeOnConfirm: false,
        animation: "slide-from-top",
        inputPlaceholder: "Team",
        inputValue: "",
        showLoaderOnConfirm: true
    },
    function (inputValue) {
        if (inputValue === false) return false;

        if (inputValue === "") {
            swal.showInputError("Name can not be empty!");
            return false
        }
        var myurl = "/Team/Create/";
        var mydata = new Object();
        mydata = {
            'TeamName': inputValue
        }

        loader('show');
        $.ajax({
            type: "POST",
            url: myurl,
            data: mydata,
            dataType: "json",
            success: function (result) {
                loader('hide');
                if (result.IsSuccess) {
                    furnishDatatable(result.data);
                    swal({
                        title: "",
                        text: "Team has been created.",
                        timer: 3000,
                        type: "success"
                    });
                }
                else {
                    swal("Failed!", result.msg, "error");
                    //showNotification(result.msg, "error");
                }

            },
            failure: function (response) {
                loader('hide');
                swal("Failed!", "Error occured while creating team!", "error");
            },
            error: function (response) {
                loader('hide');
                swal("Failed!", "Error occured while creating team!", "error");
            }
        });

    });

}



function editTeam(element) {
    var id = $(element).attr('data-id');
    var text = $(element).attr('data-text');

    swal({
        title: "Edit team name",
        text: "",
        type: "input",
        showCancelButton: true,
        closeOnConfirm: false,
        animation: "slide-from-top",
        inputPlaceholder: "Team",
        inputValue: text,
        showLoaderOnConfirm: true
    },
    function (inputValue) {
        if (inputValue === false) return false;

        if (inputValue === "") {
            swal.showInputError("Name can not be empty!");
            return false
        }
        var myurl = "/Team/Edit/";
        var _data = new Object();
        _data = {
            'Id': id,
            'TeamName': inputValue
        }

        loader('show');
        $.ajax({
            type: "POST",
            url: myurl,
            data: _data,
            dataType: "json",
            success: function (result) {
                loader('hide');
                if (result.IsSuccess) {
                    furnishDatatable(result.data);
                    swal({
                        title: "Team has been updated.",
                        text: "",
                        timer: 3000,
                        type: "success"
                    });
                }
                else {
                    swal("Failed!", result.msg, "error");
                    //showNotification(result.msg, "error");
                }

            },
            failure: function (response) {
                loader('hide');
                swal("Failed!", "Error occured while updating team!", "error");
            },
            error: function (response) {
                loader('hide');
                swal("Failed!", "Error occured while updating team!", "error");
            }
        });

    });

}

function deleteTeam(element) {
    var id = $(element).attr('data-id');
    var text = $(element).attr('data-text');
    swal({
        title: "Are you sure?",
        text: "Team will be deleted.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!",
        closeOnConfirm: false
    },
    function () {
        var _data = {
            'Id': id,
            'TeamName': text
        };
        $.ajax({
            type: "POST",
            url: "/Team/Delete",
            data: _data,
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    furnishDatatable(response.data);
                    swal("Deleted!", "Team has been deleted.", "success");
                }
                else {
                    swal("Error!", response.msg, "error");
                }
            },
            failure: function (response) {
                swal("Failed!", "Error occured while deleting team!", "error");
            },
            error: function (response) {
                swal("Failed!", "Error occured while deleting team!", "error");
            }
        });
    });
}
