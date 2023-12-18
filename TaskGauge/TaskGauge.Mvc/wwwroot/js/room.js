const connection = new signalR.HubConnectionBuilder()
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

connection.start().then(function () {
    connection.invoke("joinRoom", roomNameFromUrl);
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("userLeft", function (username) {
    $('#participants li:contains("' + username + '")').remove();
});

function openTaskModal() {
    $('#taskModal').modal('show');
}

function setTaskDuration() {
    let taskDuration = $('#taskDuration').val();
    let selectedTask = $('.task.active');
    selectedTask.find('.task-duration').text(' - ' + taskDuration + ' point(s)');
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

function addedNewTask(taskModel, isAdmin) {
    if (taskModel.isSuccess) {
        let tableButtonContent = getTableContentAccordingToUserType(isAdmin, taskModel.taskName);
        let tableRow =
            `<tr>
            <td>${taskModel.taskName}</td>
            <td>Open</td>
            ${tableButtonContent.detailButton}
            ${tableButtonContent.deleteButton}
            </tr>`;

        $('#taskHistoryTable').append(tableRow);
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
    let model = { detailButton: null, deleteButton: null };

    if (isAdmin) {
        model.detailButton = `<td><button type="button" class="btn btn-primary btn-sm" onclick="openTaskModal(${taskName})">Detail</button></td>`
        model.deleteButton = `<td><button type="button" class="btn btn-primary btn-sm" onclick="openTaskModal(${taskName})">Delete</button></td>`
    }
    else {
        model.detailButton = `<td><button type="button" class="btn btn-primary btn-sm" onclick="openTaskModal(${taskName})">Add Effort</button></td>`
    }
    return model;
}

function isExistTaskName(taskName) {
    return false;
}
