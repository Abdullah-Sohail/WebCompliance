var currentPage = 1;
var sortAsc = true;
var lastSort = null;
var timer;

function init(data) {
    if (data) {
        furnishDatatable(data);
    }

    $(".searchUsers").keydown(function () {
        clearTimeout(timer);  //clear any running timeout on key up
        timer = setTimeout(function () { //then give it a second to see if the user is finished
            getUsers();
        }, 500);
    });

    $("#page").click(function () {
        return false;
    });

    $('body').on('click', '.deleteProfile', function () {
        deleteUser(this);
    });
}

function sortUsers(field, column) {
    if (lastSort != field) {
        sortAsc = true;
    }

    $("#users-data-table thead th span").attr('class', 'pull-right icon-sort');
    var icon = sortAsc ? "icon-sort-up" : "icon-sort-down";
    $(column).find("span").removeClass("icon-sort").addClass(icon);
    getUsers(currentPage, field, sortAsc);
    sortAsc = !sortAsc;
    lastSort = field;
}

function getUsers(page, sortField, sortAsc) {
    if (!page) {
        page = 1;
    }
    var search = $(".searchUsers").val();
    var url = "/Account/Users?page=" + page + "&search=" + search + "&sortField=" + sortField + "&isAsc=" + sortAsc

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
    $("#users-data-table tbody").empty();

    $('#userRowTmpl')
        .tmpl(data.Items).appendTo('#users-data-table tbody');

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

function deleteUser(element) {
    var btn = element;
    swal({
        title: "Are you sure?",
        text: "User will be deleted!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!",
        closeOnConfirm: false
    },
    function () {
        var profileData = {
            UserId: $(btn).attr("data-id"),
            FirstName: ""
        };
        $.ajax({
            type: "POST",
            url: "/Account/DeleteProfile",
            data: profileData,
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    swal("Deleted!", "User has been deleted.", "success");
                    $(btn).closest('tr').remove();
                }
                else {
                    swal("Error!", response.Message, "error");
                }

            },
            failure: function (response) {
                //swal("Deleted!", "User has been deleted.", "error");
            },
            error: function (response) {
                //swal("Deleted!", "User has been deleted.", "error");
            }
        });
    });
}