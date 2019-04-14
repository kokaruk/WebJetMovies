import { IRoom } from './IRoom';
import { IUser } from './IUser';

export interface ISlot {
    room: IRoom;
    roomID: string;
    staff: IUser;
    staffID: string;
    startTime: string;
    student: IUser;
    studentID: string;
}
