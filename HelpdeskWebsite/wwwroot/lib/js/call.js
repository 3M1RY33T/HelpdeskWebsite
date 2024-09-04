$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
    const getAll = async (msg) => {
        try {
            $("#callList").text("Finding Call Information...");
            let response = await fetch(`/api/call`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it
                buildEmployeeList(payload);
                msg === "" ? // are we appending to an existing message
                    $("#status").text("Calls Loaded") : $("#status").text(`${msg} - Calls Loaded`);
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        }
    }; // getAll

    const buildEmployeeList = (data, usealldata = true) => {
        $("#callList").empty();
        div = $(`<div class="list-group-item text-white bg-primary row d-flex" id="status">Call Info</div>
 <div class= "list-group-item row d-flex text-center" id="heading">
 <div class="col-4 h4 text-primary">Date</div>
 <div class="col-4 h4 text-primary">For</div>
 <div class="col-4 h4 text-primary">Problem</div>
 </div>`);
        div.appendTo($("#callList"));
        usealldata ? sessionStorage.setItem("allcalls", JSON.stringify(data)) : null;
        let btn = $(`<button class="list-group-item row d-flex" id="0">...click to add call</button>`);
        btn.appendTo($("#callList"))
        data.forEach(call => {
            let dateOpened = call.dateOpened.replace('T', ' ');
            btn = $(`<button class="list-group-item row d-flex" id="${call.id}">`);
            btn.html(`<div class="col-4" id="callDate${call.dateOpened}">${dateOpened}</div>
 <div class="col-4" id="callfor${call.employeeName}">${call.employeeName}</div>
 <div class="col-4" id="callprob${call.problemDescription}">${call.problemDescription}</div>`
            );
            btn.appendTo($("#callList"));
        }); // forEach
    }; // buildEmployeeList


    $("#callList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "callList" || id === "") {
            id = e.target.id;
        } // clicked on row somewhere else
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allcalls"));
            //btn = $(`<button class="list-group-item row d-flex" id="0">...click to add employee</button>`);
            //btn.appendTo($("#employeeList"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        } else {
            return false; // ignore if they clicked on heading or status
        }
    }); // callListClick

    $('#theModal').on('shown.bs.modal', function (e) {
        checkValidation();
    });

    const update = async (e) => {
        // action button click event handler
        try {
            // set up a new client side instance of Employee
            let call = JSON.parse(sessionStorage.getItem("call"));
            // pouplate the properties
            call.problemId = parseInt($("#SelectedProblem").val());
            call.employeeId = parseInt($("#SelectedEmployee").val());
            call.techId = parseInt($("#SelectedTech").val());
            call.dateOpened = sessionStorage.getItem('dateOpened');
            //call.dateClosed = $("#TextBoxDateClosed").val() == "" ? null : $("#TextBoxDateClosed").val();
            call.dateClosed = sessionStorage.getItem('dateClosed') == "null" ? null : sessionStorage.getItem('dateClosed');
            call.notes = $("#TextBoxNotes").val();

            console.log(call);

            // send the updated back to the server asynchronously using Http PUT
            let response = await fetch("/api/call", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(call),
            });
            if (response.ok) {
                // or check for response.status
                let payload = await response.json();
                //$("#status").text(payload.msg);
                getAll(payload.msg);
            } else if (response.status !== 404) {
                // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        } // try/catch
    } // Update

    const clearModalFields = () => {
        $("#SelectedProblem").val($("#SelectedProblem option:first").val());
        $("#SelectedEmployee").val($("#SelectedEmployee option:first").val());
        $("#SelectedTech").val($("#SelectedTech option:first").val());
        $("#TextBoxDateOpened").val("");
        $("#TextBoxDateClosed").val("");
        $("#TextBoxNotes").val("");
        sessionStorage.removeItem("call");
        $("#theModal").modal("toggle");
        $('#TextBoxCloseCall').prop('checked', false);
    }; // clearModalFields

    const setupForAdd = () => {
        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>Add Call</h4>");
        $("#theModal").modal("toggle");
        $("#modalstatus").text("add new call");
        $("#theModalLabel").text("Add");
        $('#deletebutton').hide();
        clearModalFields();
        sessionStorage.setItem('dateOpened', formatDate());
        $('#TextBoxDateOpened').val(formatDate().replace('T', ' '));

        $('#SelectedProblem').prop("disabled", false);
        $('#SelectedProblem').prop("disabled", false);
        $('#SelectedEmployee').prop("disabled", false);
        $('#SelectedTech').prop("disabled", false);
        $('#TextBoxNotes').prop("disabled", false);
        $('#TextBoxCloseCall').prop('checked', false);
        $('#dateClosedRow').hide();
        $('#CloseCallRow').hide();

    }; // setupForAdd

    const setupForUpdate = (id, data) => {
        $('#CloseCallRow').show();
        console.log(id);
        console.table(data);
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h4>Update Call</h4>");
        $('#deletebutton').show();
        clearModalFields();
        data.forEach(call => {
            if (call.id === parseInt(id)) {
                console.log(call);
                $("#SelectedProblem").val(call.problemId);
                //$("#TextBoxEmployee").val(call.employee);
                $('#SelectedEmployee').val(call.employeeId);
                $("#SelectedTech").val(call.techId);
                sessionStorage.setItem('dateOpened', call.dateOpened);
                $("#TextBoxDateOpened").val(call.dateOpened.replace('T', ' '));
                if (call.dateClosed != null) {
                    $('#TextBoxCloseCall').prop('checked', true);
                    $("#TextBoxDateClosed").val(call.dateClosed.replace('T', ' '));
                }
                $("#TextBoxNotes").val(call.notes);
                sessionStorage.setItem("call", JSON.stringify(call));
                $("#modalstatus").text("update data");
                $("#theModal").modal("toggle");
                $("#theModalLabel").text("Update");

                if (call.dateClosed != null && call.dateClosed != "") {
                    $('#TextBoxCloseCall').prop('checked', true);
                    $('#dateClosedRow').show();
                } else {
                    $('#TextBoxCloseCall').prop('checked', false);
                    $('#dateClosedRow').hide();
                }

                if ($("#TextBoxCloseCall").is(':checked')) {
                    $('#SelectedProblem').prop("disabled", true);
                    $('#SelectedEmployee').prop("disabled", true);
                    $('#SelectedTech').prop("disabled", true);
                    $('#TextBoxNotes').prop("disabled", true);
                    $('#actionbutton').hide();
                } else {
                    $('#SelectedProblem').prop("disabled", false);
                    $('#SelectedProblem').prop("disabled", false);
                    $('#SelectedEmployee').prop("disabled", false);
                    $('#SelectedTech').prop("disabled", false);
                    $('#TextBoxNotes').prop("disabled", false);
                    $('#actionbutton').show();
                }
            } // if
        }); // data.forEach
    }; // setupForUpdate

    const add = async () => {
        try {
            call = new Object();
            call.problemId = parseInt($("#SelectedProblem").val());
            call.employeeId = parseInt($("#SelectedEmployee").val());
            call.techId = parseInt($("#SelectedTech").val());
            call.dateOpened = sessionStorage.getItem('dateOpened');
            call.dateClosed = null;
            call.notes = $("#TextBoxNotes").val();
            call.id = -1;
            // send the employee info to the server asynchronously using POST
            let response = await fetch("/api/call", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8"
                },
                body: JSON.stringify(call),
            });
            if (response.ok) // or check for response.status
            {
                let data = await response.json();
                getAll(data.msg);
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        } // try/catch
        $("#theModal").modal("toggle");
    }; // add

    $("#actionbutton").on("click", () => {
        if ($("#CallModalForm").valid()) {
            $("#actionbutton").val() === "update" ? update() : add();
        }
    }); // actionbutton click

    const errorRtn = (problemJson, status) => {
        if (status > 499) {
            $("#status").text("Problem server side, see debug console");
        } else {
            let keys = Object.keys(problemJson.errors)
            problem = {
                status: status,
                statusText: problemJson.errors[keys[0]][0], // first error
            };
            $("#status").text("Problem client side, see browser console");
            console.log(problem);
        } // else
    };

    //const formatDate = () => {
    //    var d = new Date();
    //    d.setSeconds(0, 0);
    //    return d.toISOString().replace('.000Z','');
    //};

    const formatDate = (date) => {
        let d;
        (date === undefined) ? d = new Date() : d = new Date(Date.parse(date));
        let _day = d.getDate();
        if (_day < 10) { _day = "0" + _day; }
        let _month = d.getMonth() + 1;
        if (_month < 10) { _month = "0" + _month; }
        let _year = d.getFullYear();
        let _hour = d.getHours();
        if (_hour < 10) { _hour = "0" + _hour; }
        let _min = d.getMinutes();
        if (_min < 10) { _min = "0" + _min; }
        return _year + "-" + _month + "-" + _day + "T" + _hour + ":" + _min;
    } // formatDate


    $("#deletebutton").on('click', async (e) => {
        try {
            let call = JSON.parse(sessionStorage.getItem("call"));

            let response = await fetch(`/api/call/${call.id}`, {
                method: "DELETE",
                headers: { "Content-Type": "application/json; charset=utf-8" },
            });
            if (response.ok) {
                let payload = await response.json();
                getAll(payload.msg);
                $("#status").text("Call Deleted"); // Set the status message to "Employee Deleted"
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("no such path on server");
            }
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        }
    });

    $("#TextBoxCloseCall").on("change", () => {
        if ($("#TextBoxCloseCall").is(':checked')) {
            $('#dateClosedRow').show();
            //$('#SelectedProblem').prop("disabled", true);
            //$('#SelectedEmployee').prop("disabled", true);
            //$('#SelectedTech').prop("disabled", true);
            //$('#TextBoxNotes').prop("disabled", true);
            $("#TextBoxDateClosed").val(formatDate().replace('T', ' '));
            sessionStorage.setItem("dateClosed", formatDate());
        } else {
            $('#dateClosedRow').hide();
            //$('#SelectedProblem').prop("disabled", false);
            $('#TextBoxDateClosed').val("");
            //$('#SelectedProblem').prop("disabled", false);
            //$('#SelectedEmployee').prop("disabled", false);
            //$('#SelectedTech').prop("disabled", false);
            //$('#TextBoxNotes').prop("disabled", false);
            sessionStorage.setItem("dateClosed", null);
        }
    }); // checkBoxClose

    //document.addEventListener("keyup", e => {
    //    console.log('ku');
    //    $("#modalstatus").removeClass(); //remove any existing css on div
    //    if ($("#CallModalForm").valid()) {
    //        $("#modalstatus").attr("class", "badge bg-primary"); //green
    //        $("#modalstatus").text("data entered is valid");
    //    }
    //    else {
    //        $("#modalstatus").attr("class", "badge bg-danger"); //red
    //        $("#modalstatus").text("fix errors");
    //    }
    //});

    $("#CallModalForm input, #CallModalForm select, #CallModalForm textarea").on('keyup focusout change', function () {
        checkValidation();
    });

    const checkValidation = () => {
        if ($("#CallModalForm").valid()) {
            $("#actionbutton").prop('disabled', false);
        } else {
            $("#actionbutton").prop('disabled', true);
        }
    };

    $("#CallModalForm").validate({
        rules: {
            SelectedProblem: { range: [1, Infinity] },
            SelectedEmployee: { range: [1, Infinity] },
            SelectedTech: { range: [1, Infinity] },
            TextBoxNotes: { maxlength: 200, required: false, }
        },
        errorElement: "div",
        messages: {
            SelectedProblem: {
                range: "please choose a problem."
            },
            SelectedEmployee: {
                range: "please choose an employee."
            },
            SelectedTech: {
                range: "please choose a technician."
            },
            TextBoxNotes: {
                maxlength: "please limit to 200 characters."
            },
        }
    }); //CallModalForm.validate



    const getAllEmployees = async () => {
        try {
            $("#callList").text("Finding Call Information...");
            let response = await fetch(`/api/employee`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it

                // SelectedEmployee
                $('#SelectedEmployee').empty();
                $('#SelectedEmployee').append($('<option>', {
                    value: '0',
                    text: 'Please Select Employee'
                }));
                $.each(payload, function (i, item) {
                    $('#SelectedEmployee').append($('<option>', {
                        value: item.id,
                        text: item.title + ' ' + item.firstname + ' ' + item.lastname
                    }));
                });
                // SelectedTech
                $('#SelectedTech').empty();
                $('#SelectedTech').append($('<option>', {
                    value: '0',
                    text: 'Please Select Technician'
                }));
                $.each(payload, function (i, item) {
                    $('#SelectedTech').append($('<option>', {
                        value: item.id,
                        text: item.title + ' ' + item.firstname + ' ' + item.lastname
                    }));
                });

                $("#status").text("Employees Loaded")
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        }
    }; // getAll

    const getAllProblems = async () => {
        try {
            let response = await fetch(`/api/problem`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it

                // SelectedEmployee
                $('#SelectedProblem').empty();
                $('#SelectedProblem').append($('<option>', {
                    value: '0',
                    text: 'Please Select Problem'
                }));
                $.each(payload, function (i, item) {
                    $('#SelectedProblem').append($('<option>', {
                        value: item.id,
                        text: item.description
                    }));
                });

                $("#status").text("Problems Loaded")
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        }
    }; // getAll

    $("#srch").on("keyup", () => {
        let alldata = JSON.parse(sessionStorage.getItem("allcalls"));
        if ($("#srch").val() == '') {
            buildEmployeeList(alldata, false);
            return;
        }
        let filtereddata = alldata.filter((call) => call.employeeName.match(new RegExp($("#srch").val(), 'i')));
        buildEmployeeList(filtereddata, false);
    }); // srch keyup

    getAll();
    getAllEmployees();
    getAllProblems();
});
