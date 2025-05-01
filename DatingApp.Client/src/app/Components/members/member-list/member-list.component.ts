import { Component, OnInit } from '@angular/core';
import { MembersService } from '../../../services/members.service';
import { MemberModel } from '../../../Models/member';
import { CommonModule } from '@angular/common';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModel } from '../../../Models/pagination';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { UserParams } from '../../../Models/userParams';
import { AccountService } from '../../../services/account.service';
import { UserModel } from '../../../Models/userModel';
import { take } from 'rxjs';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@Component({
    selector: 'app-member-list',
    standalone: true,
    imports: [CommonModule, MemberCardComponent, PaginationModule, FormsModule, ButtonsModule],
    templateUrl: './member-list.component.html',
    styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit {
    
    members: MemberModel[] = [];
    pagination?: PaginationModel;
    userParams!: UserParams;
    user?: UserModel;
    genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }];

    constructor(private memberService: MembersService) {
        this.userParams = this.memberService.getUserParams();
    }

    ngOnInit(): void {
        this.loadMembers();
    }

    loadMembers() {
        this.memberService.setUserParams(this.userParams);
        this.memberService.getMembers(this.userParams).subscribe(response => {
            this.members = response.result!;
            this.pagination = response.pagination;
        })
    }

    resetFilters() {
        this.memberService.resetUserParams();
        this.loadMembers();
    }

    pageChanged(event: any) {
        this.userParams.pageNumber = event.page;
        this.memberService.setUserParams(this.userParams);
        this.loadMembers();
    }
}
