import { Component, EventEmitter, Input } from '@angular/core';
import { BsModalRef, ModalModule } from 'ngx-bootstrap/modal';
import { UserModel } from '../../../Models/userModel';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-roles-modal',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './roles-modal.component.html',
  styleUrl: './roles-modal.component.css'
})
export class RolesModalComponent {

    @Input() updateSelectedRoles = new EventEmitter();
    user?: UserModel;
    roles: any[]  = [];

    constructor(public bsModalRef: BsModalRef){ }

    updateRoles(){
        this.updateSelectedRoles.emit(this.roles);
        this.bsModalRef.hide();
    }
}
