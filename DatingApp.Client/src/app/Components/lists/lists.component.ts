import { Component, OnInit } from '@angular/core';
import { MemberModel } from '../../Models/member';
import { MembersService } from '../../services/members.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { PaginatedResult, PaginationModel } from '../../Models/pagination';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { RouterLink } from '@angular/router';
import { MessageModel } from '../../Models/message';
import { MemberCardComponent } from '../members/member-card/member-card.component';

@Component({
    selector: 'app-lists',
    standalone: true,
    imports: [MemberCardComponent, FormsModule, ButtonsModule, PaginationModule],
    templateUrl: './lists.component.html',
    styleUrl: './lists.component.css'
})

export class ListsComponent implements OnInit {
    members?: Partial<MemberModel[]>;
    predicate = 'liked';
    pageNumber = 1;
    pageSize = 5;
    pagination?: PaginationModel;

    constructor(private memberService: MembersService) {

    }

    ngOnInit(): void {
        this.loadLikes();
    }

    loadLikes() {
        this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe((response) => {
            this.members = response.result;
            this.pagination = response.pagination;
        })
    }

    pageChanged(event: any){
        this.pageNumber = event.page;
        this.loadLikes();
    }
}
