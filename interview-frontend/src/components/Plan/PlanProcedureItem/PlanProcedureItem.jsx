import React, { useState } from "react";
import ReactSelect from "react-select";
import {
    addUserToPlanProcedure,
    removeUserFromPlanProcedure,
    removeAllUsersFromPlanProcedure,
} from "../../../api/api";

const PlanProcedureItem = ({ planId, procedure, users, planProcedureUsers }) => {
    const [selectedUsers, setSelectedUsers] = useState(() =>
        users.filter((u) =>
            planProcedureUsers.some((ppu) => ppu.userId === u.value)
        )
    );

    const handleAssignUserToProcedure = async (option) => {
        const hasUserInProcedure = selectedUsers.some((u) => u.value === option.value);
        if (hasUserInProcedure) return;

        await addUserToPlanProcedure(planId, procedure.procedureId, option.value);
        setSelectedUsers((prevState) => [...prevState, option]);
    };

    const handleRemoveUserFromProcedure = async (removedUser) => {
        // backspace on an empty select still fires pop-value, with nothing removed
        if (!removedUser) return;

        await removeUserFromPlanProcedure(planId, procedure.procedureId, removedUser.value);
        setSelectedUsers((prevState) => prevState.filter((u) => u.value !== removedUser.value));
    };

    const handleRemoveAllUsersFromProcedure = async () => {
        await removeAllUsersFromPlanProcedure(planId, procedure.procedureId);
        setSelectedUsers([]);
    };

    // state is only updated after the api call succeeds, so the select never shows something that was not saved
    const handleUsersChange = (e, { action, option, removedValue }) => {
        switch (action) {
            case "select-option":
                return handleAssignUserToProcedure(option);
            case "remove-value":
            case "pop-value":
                return handleRemoveUserFromProcedure(removedValue);
            case "clear":
                return handleRemoveAllUsersFromProcedure();
            default:
                return;
        }
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e, actionMeta) => handleUsersChange(e, actionMeta)}
            />
        </div>
    );
};

export default PlanProcedureItem;
