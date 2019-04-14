import {Component, OnInit} from '@angular/core';
import {RoomService} from '../../../services/room.service';
import {IRoom} from '../../../models/IRoom';
import * as lodash from 'lodash';
import {IUser} from '../../../models/IUser';
import {UserService} from '../../../services/user.service';
import {ISlot} from '../../../models/ISlot';
import {BookingService} from '../../../services/booking.service';


@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.sass']
})
export class RoomComponent implements OnInit {
  rooms: Array<IRoom> = [];
  users: Array<IUser> = [];
  students: Array<IUser> = [];
  staff: Array<IUser> = [];
  roomID: string [];

  constructor(
    private roomService: RoomService,
    private bookingService: BookingService,
    private userService: UserService
  ) {
  }

  ngOnInit() {
    this.getAllRooms();
    this.getAllUsers();
  }

  private getAllRooms(): void {
    this.roomService.getRooms().subscribe(
      rooms => {
        this.rooms = rooms;
      }
    );
  }

  private getAllUsers(): void {
    this.userService.getAllUsers().subscribe(users => {
      this.users = users;
      this.students = lodash.filter(this.users, user => user.userID.charAt(0) === 's'); // TODO move this to user service
      this.staff = lodash.filter(this.users, user => user.userID.charAt(0) === 'e'); // TODO move this to user service
    });
  }

  addRoom(roomID: string) {
    const room = <IRoom>{
      roomID: roomID,
      slots: []
    };
    this.roomService.addRoom(room).subscribe(newRoom => {
      this.rooms.push(newRoom);
    });
  }


  deleteRoom(room: IRoom) {
    this.roomService.deleteRoom(room).subscribe(r => {
      console.log(r.roomID);
      const ii = this.rooms.findIndex(rm => rm.roomID === r.roomID);
      this.rooms.splice(ii, 1);
    });
  }

  public editSlot(slot: ISlot): void {
    console.log(`editing slot`);
    if (!slot.studentID) {
      slot.student = null;
      slot.studentID = null;
    }
    const upSlot: ISlot = {
      room: slot.room,
      roomID: slot.roomID,
      staff: slot.staff,
      staffID: slot.staffID,
      startTime: slot.startTime,
      student: slot.student,
      studentID: slot.studentID
    };
    this.bookingService.editSlot(upSlot).subscribe();
  }

  onStudentChange(slot: ISlot) {
    const student = this.users.find(s => s.userID === slot.studentID);
    if (student) {
      console.log(student.userID + ' ' + student.name + ' ' + student.email);
      slot.studentID = student.userID;
      slot.student = student;
    } else {
      slot.studentID = null;
      slot.student = null;
    }
  }


  // TODO no idea what this does
  editRoom(room: IRoom) {
    this.roomService.editRoom(room).subscribe(editedRoom => {
      this.rooms = lodash.map(this.rooms, currentRoom => {
        (currentRoom.roomID === editedRoom.roomID) ? editedRoom : currentRoom;
      });
    });
  }

}
