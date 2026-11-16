import { Interview } from './interview';

/** Numeric values mirror the backend PositionStatus enum, which is serialized as a number. */
export const POSITION_STATUS = {
     Applied: 0,
     InProgress: 1,
     Passed: 2,
     Failed: 3,
} as const;

export type PositionStatus = (typeof POSITION_STATUS)[keyof typeof POSITION_STATUS];

//record is used to map PositionStatus values to their corresponding labels , key value pairs for each status
export const POSITION_STATUS_LABELS: Record<PositionStatus, string> = {
     [POSITION_STATUS.Applied]: 'Applied',
     [POSITION_STATUS.InProgress]: 'In progress',
     [POSITION_STATUS.Passed]: 'Passed',
     [POSITION_STATUS.Failed]: 'Failed',
};

// Provides a list of options for dropdowns or select inputs, mapping each PositionStatus to its label.
export const POSITION_STATUS_OPTIONS: readonly { value: PositionStatus; label: string }[] = (
     Object.values(POSITION_STATUS) as PositionStatus[]
).map((value) => ({
     value,
     label: POSITION_STATUS_LABELS[value],
}));

export function positionStatusLabel(status: PositionStatus): string {
     return POSITION_STATUS_LABELS[status] ?? String(status);
}

export interface Position {
     id: string;
     title: string;
     applyDate: string;
     status: PositionStatus;
     notes?: string;
     companyId: string;
     interviews?: Interview[];
}