import {Component, OnInit} from '@angular/core';
import {BookingService} from '../../../services/booking.service';
import {ISlot} from '../../../models/ISlot';
import * as lodash from 'lodash';
import {IUser} from '../../../models/IUser';
import {UserService} from '../../../services/user.service';

@Component({
  selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.sass']
})
export class BookingComponent implements OnInit {
  slots: Array<ISlot> = [];
  displaySlots: Array<ISlot> = [];
  users: Array<IUser> = [];
  students: Array<IUser> = [];
  staff: Array<IUser> = [];
  staffToggle: boolean;
  selectedUser: IUser;

  constructor(
    private bookingService: BookingService,
    private userService: UserService,
  ) {
    this.staffToggle = false;
  }

  ngOnInit() {
    // get the slots
    this.getAllUsers();
    this.getAllSlots();
    this.modifyDisplay();
  }

  deleteSlot(slot: ISlot): void {
    console.log(`deleting slot`, slot);
    this.bookingService.deleteSlot(slot).subscribe( s => {
      console.log('for staff: ' + s.staffID);
      const i = this.slots.findIndex(ss => this.slotPrimaryKeyPredicate(ss, s) );
      console.log('deleteing at index ' + i);
      this.slots.splice(i, 1);
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

  toggleStaff(): void {
    this.staffToggle = !this.staffToggle;
    this.selectedUser = undefined;
  }

  selectUser($event) {
    this.selectedUser = $event.value;
    this.modifyDisplay();
  }

  private slotPrimaryKeyPredicate(slot1: ISlot, slot2: ISlot): boolean {
    return slot1.roomID === slot2.roomID && slot1.startTime === slot2.startTime;
  }

  private getAllSlots(): void {
    this.bookingService.getAllSlots().subscribe(
      slots => {
        this.slots = slots;
        this.displaySlots = slots;
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

  private modifyDisplay(): void {
    (this.selectedUser !== undefined) ?
      this.displaySlots = lodash.filter(this.slots, slot => slot.staffID === this.selectedUser || slot.studentID === this.selectedUser)
      : this.displaySlots = this.slots;
  }
}
