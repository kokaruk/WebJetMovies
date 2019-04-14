import { ISlot } from './ISlot';

export interface IRoom {
    roomID?: string;
    slots: Array<ISlot>;
}
