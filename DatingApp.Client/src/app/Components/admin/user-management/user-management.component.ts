import { Component, OnInit } from '@angular/core';
import { UserModel } from '../../../Models/userModel';
import { AdminService } from '../../../services/admin.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

@Component({
    selector: 'app-user-management',
    standalone: true,
    imports: [],
    templateUrl: './user-management.component.html',
    styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {

    users?: Partial<UserModel[]>;
    bsModalRef?: BsModalRef;

    constructor(private adminService: AdminService, private modalService: BsModalService) { }

    ngOnInit(): void {
        this.getUserWithRoles();
    }

    getUserWithRoles() {
        this.adminService.getUserWithRoles().subscribe(users => {
            this.users = users;
        })
    }

    openRolesModal(user: UserModel) {
        const config = {
            class: 'modal-dialog-centered',
            initialState: {
                user,
                roles: this.getRolesArray(user)
            }
        }

        this.bsModalRef = this.modalService.show(RolesModalComponent, config);
        this.bsModalRef.content.updateSelectedRoles.subscribe((values: any[]) => {
            const rolesToUpdate = {
                roles: [...values.filter(el => el.checked === true).map(el => el.name)]
            };

            if (rolesToUpdate) {
                this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(() => {
                    user.roles = [...rolesToUpdate.roles]
                });
            }
        })
    }

    private getRolesArray(user: UserModel) {
        const roles: string[] = [];
        const userRoles = user.roles;
        const availableRoles: any[] = [
            { name: 'Admin', value: 'Admin' },
            { name: 'Moderator', value: 'Moderator' },
            { name: 'Member', value: 'Member' },
        ]

        availableRoles.forEach(role => {
            let isMatch = false;
            for (const index in userRoles) {
                if (role.name === userRoles[index]) {
                    isMatch = true;
                    role.checked = true;
                    roles.push(role);
                    break;
                }
            }

            if (!isMatch) {
                role.checked = false;
                roles.push(role);
            }

        });

        return roles;
    }
}
