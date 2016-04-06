$(function () {
    var userName = null, tips = $(".validateTips"),
        txtUserName = $("#userName");

    var dialog = $("#dialog-form").dialog({
        autoOpen: true,
        width: 470,
        closeOnEscape: false,
        dialogClass: 'no-close',
        height: 200,
        modal: true,
        buttons: {
            "Login": doLogin
        }
    });

    var dlgEditX = $("#dlgEditXUser").dialog({
        title: "Edit X User",
        autoOpen: false,
        closeOnEscape: false,
        width: 350,
        modal: true,
        buttons: {
            "Save": btnSaveX,
            "Delete": deleteX,
            "Cancel": dlgXclose
        }
    });

    var dlgEditY = $("#dlgEditYUser").dialog({
        title: "Edit Y User",
        autoOpen: false,
        closeOnEscape: false,
        width: 350,
        modal: true,
        buttons: {
            "Save": saveY,
            "Delete": deleteY,
            "Cancel": dlgYclose,
        }
    });

    var dlgResolveXUser = $("#dlgResolveXUser").dialog({
        title: "Resolve X User conflict",
        autoOpen: false,
        closeOnEscape: false,
        width: 350,
        modal: true,
        buttons: {
            "Apply": resolveY,
            "Cancel": dlgYcancelResolving,
        }
    });

    var dlgNewUser = $("#dlgNewUser").dialog({
        autoOpen: false,
        width: 350,
        modal: true
    });

    function doLogin() {
        var valid = true;
        var allFields = $([]).add(txtUserName);
        allFields.removeClass("ui-state-error");

        valid = valid && checkLength(txtUserName, "userName", 3, 16);
        if (valid) {
            userName = txtUserName.val();
            dialog.dialog("close");
            $('.userName').html("&#64;" + htmlEncode(userName));
            $('.userName').data("loggedId", true);
        }
        return valid;
    };

    function getXUsers() {
        $.ajax("/api/users/x", {
            type: "GET",
            success: function (data) {
                console.log(data);
                $('#validationSummaryX').html('');
                $("#xUsers tbody").html('');
                $("#tmpXCell").tmpl(data).appendTo("#xUsers tbody");
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryX').html(messages);
            }
        });
    };

    function getYUsers() {
        $.ajax("/api/users/y", {
            type: "GET",
            success: function (data) {
                console.log(data);
                $('#validationSummaryY').html('');
                $("#yUsers tbody").html('');
                $("#tmpYCell").tmpl(data).appendTo("#yUsers tbody");
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryY').html(messages);
            }
        });
    };

    function createXUser() {
        var fname = $("#firstName");
        var lname = $("#lastName");

        var valid = validateField("firstName") && validateField("lastName");
        if (!valid) return;

        $.ajax("/api/user/x", {
            data: {
                Id: 0,
                FirstName: fname.val(),
                LastName: lname.val()
            },
            dataType: "json",
            type: "PUT",
            success: function (data) {
                console.log(data);
                $('#validationSummaryX').html('');

                var user = {
                    Id: data.Id,
                    EpochUpdateDate: data.EpochUpdateDate,
                    FirstName: fname.val(),
                    LastName: lname.val()
                };

                $("#tmpXCell").tmpl(user).appendTo("#xUsers tbody");
                fname.val('');
                lname.val('');
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryX').html(messages);
            }
        });
       
        dlgNewUser.dialog("close");
        event.preventDefault();
    }

    function createYUser() {
        var fname = $("#firstName");
        var lname = $("#lastName");

        var valid = validateField("firstName") && validateField("lastName");
        if (!valid) return;

        $.ajax("/api/user/y", {
            data: {
                Id: 0,
                FirstName: fname.val(),
                LastName: lname.val()
            },
            dataType: "json",
            type: "PUT",
            success: function (data) {
                console.log(data);
                $('#validationSummaryY').html('');

                var user = {
                    Id: data,
                    FirstName: fname.val(),
                    LastName: lname.val(),
                    OwnerId: "",
                    IsBlocked: false
                };

                $("#tmpYCell").tmpl(user).appendTo("#yUsers tbody");
                fname.val('');
                lname.val('');
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryY').html(messages);
            }
        });

        dlgNewUser.dialog("close");
        event.preventDefault();
    }

    function btnSaveX() {
        var id = $("#idX").val();
        var fname = $("#firstXName");
        var lname = $("#lastXName");
        var updateDate = +$("#hdnUpdateDate").val();

        var valid = validateField("firstXName") && validateField("lastXName");
        if (!valid) return;
        var data = {
            Id: id,
            FirstName: fname.val(),
            LastName: lname.val(),
            EpochUpdateDate: updateDate
        };

        saveXUser(data);

        dlgEditX.dialog("close");
    }

    function saveXUser(user) {
        $.ajax("/api/user/x", {
            data: user,
            dataType: "json",
            type: "POST",
            success: function (data) {
                console.log(data);
                $('#validationSummaryX').html('');

                if (data.Id === -1) {
                    alert("Sorry, this user already deleted.");
                    getXUsers();
                    return;
                }

                if (data.EpochUpdateDate === user.EpochUpdateDate) {
                    compareAndFixChanges(data);
                } else {
                    getXUsers();
                }
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryX').html(messages);
            }
        });
    }

    function resolveY() {

        var confr = $('input:radio[name=rbConflict]:checked').val();
        if (confr == null) {
            alert('Please select an option!');
            return false;
        }
        var data = {
            Id: +$("#hdnUserId").val(),
            EpochUpdateDate: +$("#hdnUpdateDate").val()
        }

        switch (confr) {
            case "1":
                data.FirstName = $("#firstNameA").html();
                data.LastName = $("#lastNameB").html();
                break;
            case "2":
                data.FirstName = $("#firstNameB").html();
                data.LastName = $("#lastNameA").html();
                break;
            case "3":
                data.FirstName = $("#firstNameA").html();
                data.LastName = $("#lastNameA").html();
                break;
        }

        saveXUser(data);

        dlgResolveXUser.dialog("close");
        $('input:radio[name=rbConflict]:checked').prop('checked', false);
        return false;
    }

    function dlgYcancelResolving() {
        dlgResolveXUser.dialog("close");
        return false;
    }

    function compareAndFixChanges(user) {
        $.ajax("/api/user/x/" + user.Id, {
            type: "GET",
            success: function (data) {
                $('#validationSummaryX').html('');

                if (data.IsDeleted === true) {
                    alert("Sorry, this user already deleted.");
                    return false;
                }

                dlgResolveXUser.dialog("open");

                $("#hdnUserId").val(user.Id);
                $("#hdnUpdateDate").val(data.EpochUpdateDate);

                $("#firstNameA").html(user.FirstName);
                $("#lastNameA").html(user.LastName);
                $("#versionA").html(new Date(parseFloat(user.EpochUpdateDate)));

                $("#firstNameB").html(data.FirstName);
                $("#lastNameB").html(data.LastName);
                $("#versionB").html(new Date(parseFloat(data.EpochUpdateDate)));

                return false;
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryX').html(messages);
            }
        });
    }

    function saveY() {
        var id = $("#idY").val();
        var owner = $("#hdnOwner").val();

        var fname = $("#firstYName");
        var lname = $("#lastYName");

        var valid = validateField("firstYName") && validateField("lastYName");
        
        if (!valid) return;

        $.ajax("/api/user/y", {
            data: {
                Id: id,
                OwnerId: owner,
                FirstName: fname.val(),
                LastName: lname.val()
            },
            dataType: "json",
            type: "POST",
            success: function (data) {
                $('#validationSummaryY').html('');

                if (data === -1) {
                    alert("Sorry, this user already locked. Pls try again later.");
                }

                getYUsers();
            },
            error: function (e, s) {
                getYUsers();
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryY').html(messages);
            }
        });

        dlgEditY.dialog("close");
    }

    function deleteX() {
        var id = $("#idX").val();
        var updateDate = $("#hdnUpdateDate").val();
        $.ajax("/api/user/x", {
            data: {
                id: id,
                epochUpdateDate: +updateDate
            },
            dataType: "json",
            type: "DELETE",
            success: function (data) {
                $('#validationSummaryX').html('');
                if (data === 0) getXUsers();
                else if (data === -1) {
                    alert('Version of Databse record changed! Pls update records and try again.');
                }
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryX').html(messages);
            }
        });

        dlgEditX.dialog("close");
    }

    function deleteY() {
        var id = $("#idY").val();
        var owner = $("#hdnOwner").val();
        $.ajax("/api/user/y", {
            data: {
                Id: id,
                OwnerId: owner
            },
            dataType: "json",
            type: "DELETE",
            success: function (data) {
                $('#validationSummaryY').html('');

                if (data === 0) getYUsers();
                else if (data === -1) {
                    alert("Sorry, this user already locked. Pls try again later.");
                }
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryY').html(messages);
            }
        });

        dlgEditY.dialog("close");
    }

    function dlgXclose() {
        dlgEditX.dialog("close");
    }

    function dlgYclose() {
        var id = $("#idY").val();

        $.ajax("/api/user/y/" + id + "/release/owner/" + userName, {
            type: "POST",
            success: function (data) {
                dlgEditY.dialog("close");
                $('#validationSummaryY').html('');

                if (data === -1) {
                    return;
                }

                var tr = $('#yUsers tbody tr[id=' + id + ']');
                tr.removeClass('blocked');
                tr.attr('blocked', false);
            },
            error: function (e, s) {
                dlgEditY.dialog("close");
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryY').html(messages);
            }
        });
    }

    //Edit X user handler
    $('body').on('click', '#xUsers tbody tr', function () {
        var id = $(this).attr('id');
        var fname = $(this).attr('fname');
        var lname = $(this).attr('lname');
        var updateDate = $(this).attr('updateDate');

        dlgEditX.dialog("open");

        $("#idX").val(id);
        $("#firstXName").val(fname);
        $("#lastXName").val(lname);
        $("#hdnUpdateDate").val(updateDate);

        event.preventDefault();
    });

    //Edit Y user handler
    $('body').on('click', '#yUsers tbody tr', function () {
        var id = $(this).attr('id');
        var blocked = $(this).attr('blocked');
        var owner = $(this).attr('owner');
        if (owner !== userName && blocked === "true") return;

        $.ajax("/api/user/y/" + id + "/edit/owner/" + userName, {
            type: "GET",
            success: function (data) {
                $('#validationSummaryY').html('');

                if (data.IsBlocked === true) {
                    alert("Sorry, this user already locked. Pls try again later.");
                    getYUsers();
                    return;
                }

                dlgEditY.dialog("open");

                $("#idY").val(data.Id);
                $("#firstYName").val(data.FirstName);
                $("#lastYName").val(data.LastName);
                $("#hdnOwner").val(data.OwnerId);
            },
            error: function (e, s) {
                console.log(e);
                var messages = populateErrors(e.responseJSON);
                $('#validationSummaryY').html(messages);
            }
        });

        event.preventDefault();
    });

    //Add X user handler
    $('#addX').on('click', function () {
        dlgNewUser.dialog("open").dialog({
            title: "Create X User",
            buttons: {
                "Save": createXUser
            }
        });

        event.preventDefault();
    });

    //Add Y user handler
    $('#addY').on('click', function () {
        dlgNewUser.dialog("open").dialog({
            title: "Create Y User",
            buttons: {
                "Save": createYUser
            }
        });

        event.preventDefault();
    });

    $('#refreshX').on('click', function () {
        getXUsers();
        event.preventDefault();
    });

    $('#refreshY').on('click', function () {
        getYUsers();
        event.preventDefault();
    });

    function validateField(fieldName) {
        var field = $("#" + fieldName);
        var allFields = $([]).add(field);
        allFields.removeClass("ui-state-error");
        return checkLength(field, fieldName, 3, 16);
    }

    function populateErrors(responseJSON) {
        var message = responseJSON.ExceptionMessage;
        if (responseJSON.ModelState != null) {
            message = "";
            $.each(responseJSON.ModelState, function (i, fieldItem) {
                message += i;
                message += ' : ';
                message += fieldItem;
                message += "\n";
            });
        }
        return message;
    }

    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }

    function updateTips(t) {
        tips
        .text(t)
        .addClass("ui-state-highlight");
        setTimeout(function () {
            tips.removeClass("ui-state-highlight", 1500);
        }, 500);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass("ui-state-error");
            updateTips("Length of " + n + " must be between " +
                min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    };

    $("#dialog-form").submit(function (event) {
        doLogin();
        event.preventDefault();
    });

    dialog.dialog("open");
    getYUsers();
    getXUsers();
});