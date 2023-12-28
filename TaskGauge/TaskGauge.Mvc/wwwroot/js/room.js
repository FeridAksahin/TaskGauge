const connection = new signalR.HubConnectionBuilder()
    .withUrl("/taskGaugeHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("userJoined", function (username) { 
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
let openedTaskNameOfTheEffortModal;
function openTaskModal(taskName) {
    openedTaskNameOfTheEffortModal = taskName;
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
let taskEfforts;
let openedTaskName;
function taskDetail(taskName) {
    openedTaskName = taskName;
    let tds = document.querySelectorAll("#taskDetailModal td");


    tds.forEach(function (td) {
        td.parentNode.removeChild(td);
    });

    $('#taskDetailModal').modal('show');
    if (taskEfforts != null) {
        for (const element of taskEfforts) {
            if (taskName == element.taskName) {
                let tableRow =
                    `<tr>
                 <td>${element.username}</td> 
                 <td id = "${element.username}">${element.effort}</td>  
                </tr>`;

                $('#taskDetailTable').append(tableRow);

            }
        }
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

    taskEfforts = taskEffortList;

    if ($("#taskDetailModal").css("display") === "block") {
        for (let item = 0; item < taskEfforts.length; item++) {
            if (document.getElementById(`${taskEfforts[item].username}`) != null && openedTaskName == taskEfforts[item].taskName) {
                document.getElementById(`${taskEfforts[item].username}`).innerHTML = taskEfforts[item].effort
            } else if (openedTaskName == taskEfforts[item].taskName) {
                let tableRow =
                    `<tr>
                 <td>${taskEfforts[item].username}</td> 
                 <td id = "${taskEfforts[item].username}">${taskEfforts[item].effort}</td>  
                </tr>`;

                $('#taskDetailTable').append(tableRow);
            }
        }
    }
}) 

connection.on("addTaskForJoinedUser", function (taskList) {
    
    let booleanValueIsAdmin = JSON.parse(isAdmin);
    for (const element of taskList) {
        let tableButtonContent = getTableContentAccordingToUserType(booleanValueIsAdmin, element.taskName, element.taskSituation);
        let tableRow =
            `<tr>
            <td>${element.taskName}</td>
            ${tableButtonContent.openCloseTaskButton}
            ${tableButtonContent.detailOrButton}
            ${tableButtonContent.deleteButton}
            </tr>`;

        if (booleanValueIsAdmin) {
            $('#taskHistoryTable').append(tableRow);
        }
        else {
            $('#taskHistoryTableForNormalUser').append(tableRow);
        }
    }
})

connection.on("allTaskEffortForJoinedAdminUser", function (allTaskEffortList) {
    taskEfforts = allTaskEffortList;
})

function addedNewTask(taskModel, isAdmin) {
    if (taskModel.isSuccess) {
        let tableButtonContent = getTableContentAccordingToUserType(isAdmin, taskModel.taskName);
        let tableRow =
            `<tr>
            <td>${taskModel.taskName}</td>
            ${tableButtonContent.openCloseTaskButton}
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

function getTableContentAccordingToUserType(isAdmin, taskName, taskSituationForJoinedNewUser = null) {
    let model = { detailOrButton: null, deleteButton: null, openCloseTaskButton: null };

    if (isAdmin) {
        model.detailOrButton = `<td><button type="button" class="btn btn-primary btn-sm" onclick = "taskDetail('${taskName}')">Detail</button></td>`
        model.deleteButton = `<td><button type="button" class="btn btn-primary btn-sm">Delete</button></td>`
        model.openCloseTaskButton = `<td><button type="button" id = "openCloseTaskButton'${taskName}'" onclick="openCloseTaskForEffort('${taskName}')" class="btn btn-primary btn-sm">Close</button></td>`

        if (taskSituationForJoinedNewUser != null) {
            taskSituationForJoinedNewUser = taskSituationForJoinedNewUser == "Open" ? "Close" : "Open";
            model.openCloseTaskButton = `<td><button type="button" id = "openCloseTaskButton'${taskName}'" onclick="openCloseTaskForEffort('${taskName}')" class="btn btn-primary btn-sm">${taskSituationForJoinedNewUser}</button></td>`
        }

    }
    else {
        var isDisabled = taskSituationForJoinedNewUser == "Open" || taskSituationForJoinedNewUser == null ? '' : 'disabled';
        model.detailOrButton = `<td><button type="button" id="addEffortTask${taskName}" class="btn btn-primary btn-sm" onclick="openTaskModal('${taskName}')" ${isDisabled}>Add Effort</button></td>`
    }
    return model;
}

function openCloseTaskForEffort(taskName) {
    let button = document.getElementById(`openCloseTaskButton'${taskName}'`);
    let situation;
    if (button.innerText == "Open") {
        button.innerText = "Close";
        situation = "Open";
    } else {
        situation = "Close";
        button.innerText = "Open"
    }
    connection.invoke("openOrCloseTask", situation, taskName)
}

connection.on("changeTaskSituation", function (situation, taskName) {
    let effortButton = document.getElementById(`addEffortTask${taskName}`);
    if (situation == "Close") {
        effortButton.disabled = true;
        if ($("#taskModal").css("display") == "block" && openedTaskNameOfTheEffortModal == taskName) {
            $('#taskModal').modal('hide')
        }
    } else {
        effortButton.disabled = false;
       
    }
})

function isExistTaskName(taskName) {
    return false;
}
