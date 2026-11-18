import React, { useState } from "react";
import ReactSelect from "react-select";
import { addUserToPlanProcedure } from "../../../api/api";

const PlanProcedureItem = ({ planId, procedure, users, planProcedureUsers }) => {
    const [selectedUsers, setSelectedUsers] = useState(() =>
        users.filter((u) =>
            planProcedureUsers.some((ppu) => ppu.userId === u.value)
        )
    );

    const handleAssignUserToProcedure = async (e, { action, option }) => {
        if (action !== "select-option") return;

        const hasUserInProcedure = selectedUsers.some((u) => u.value === option.value);
        if (hasUserInProcedure) return;

        await addUserToPlanProcedure(planId, procedure.procedureId, option.value);
        setSelectedUsers((prevState) => [...prevState, option]);
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
                onChange={(e, actionMeta) => handleAssignUserToProcedure(e, actionMeta)}
            />
        </div>
    );
};

export default PlanProcedureItem;
