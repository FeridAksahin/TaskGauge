﻿const connection = new signalR.HubConnectionBuilder()
    .withUrl("/taskGaugeHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("userJoined", function (username) {
    console.log('User joined: ' + username);
    debugger;
    $('#participants').append('<li class="participant">' + username + '</li>');
});

connection.on("userList", function (roomUserList) {
    for (var item = 0; item < roomUserList.length; item++) {
        $('#participants').append('<li class="participant">' + roomUserList[item] + '</li>');
    }
});

let urlParams = new URLSearchParams(window.location.search);
let roomNameFromUrl = urlParams.get('roomName');
let isAdmin = document.getElementById('isAdmin').value;

connection.start().then(function () {
    connection.invoke("joinRoom", roomNameFromUrl, isAdmin);
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("userLeft", function (username) {
    $('#participants li:contains("' + username + '")').remove();
});

function openTaskModal(taskName) {
    $('#taskModal').modal('show');

    let setDurationButton = document.querySelector('#taskModal .modal-body .btn-success');
    if (setDurationButton) { 
        setDurationButton.onclick = function () {
            setTaskDuration(taskName);
        };
    } else { 
        $('#taskModal .modal-body').append(`<button type="button" class="btn btn-success" onclick="setTaskDuration('${taskName}')">Set Duration</button>`);
    }

}

function setTaskDuration(taskName) {
    let taskEffortDuration = $('#taskDuration').val();
    connection.invoke("taskEffort", taskName, taskEffortDuration);
    $('#taskModal').modal('hide');
}

function updateParticipants(participantList) {
    $('#participants').empty();
    participantList.forEach(function (participant) {
        $('#participants').append('<li class="participant">' + participant + '</li>');
    });
}

function addTask() {
    let taskName = $('#taskName').val();

    if (taskName.trim() !== "" && !isExistTaskName(taskName)) {
        connection.invoke("addTask", taskName);
    }
} 

connection.on("addedTaskByAdmin", function (taskModel) { 
    addedNewTask(taskModel, true);

});

connection.on("newTask", function (taskModel) {
    addedNewTask(taskModel, false);
})

connection.on("getEffort", function (taskEffortList) {
    debugger;
})

function addedNewTask(taskModel, isAdmin) {
    if (taskModel.isSuccess) {
        let tableButtonContent = getTableContentAccordingToUserType(isAdmin, taskModel.taskName);
        let tableRow =
            `<tr>
            <td>${taskModel.taskName}</td>
            <td>Open</td>
            ${tableButtonContent.detailOrButton}
            ${tableButtonContent.deleteButton}
            </tr>`;

        if (isAdmin) {
            $('#taskHistoryTable').append(tableRow);
        }
        else { 
            $('#taskHistoryTableForNormalUser').append(tableRow);
        }
    }
    else {
        Swal.fire({
            text: taskModel.message,
            icon: 'error',
            confirmButtonText: 'OK'
        })
    }
}

function getTableContentAccordingToUserType(isAdmin, taskName) {
    let model = { detailOrButton: null, deleteButton: null };

    if (isAdmin) {
        model.detailOrButton = `<td><button type="button" class="btn btn-primary btn-sm">Detail</button></td>`
        model.deleteButton = `<td><button type="button" class="btn btn-primary btn-sm">Delete</button></td>`
    }
    else {
        model.detailOrButton = `<td><button type="button" class="btn btn-primary btn-sm" onclick="openTaskModal('${taskName}')">Add Effort</button></td>`
    }
    return model;
}

function isExistTaskName(taskName) {
    return false;
}
