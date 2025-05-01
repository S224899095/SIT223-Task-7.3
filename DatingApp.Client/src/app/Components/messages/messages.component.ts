import { Component, OnInit } from '@angular/core';
import { MessageService } from '../../services/message.service';
import { MessageModel } from '../../Models/message';
import { PaginationModel } from '../../Models/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ConfirmService } from '../../services/confirm.service';

@Component({
    selector: 'app-messages',
    standalone: true,
    imports: [FormsModule, ButtonsModule, RouterLink, CommonModule, PaginationModule],
    templateUrl: './messages.component.html',
    styleUrl: './messages.component.css'
})

export class MessagesComponent implements OnInit {

    messages: MessageModel[] = [];
    pagination!: PaginationModel;
    container: string = 'Unread';
    pageNumber: number = 1;
    pageSize: number = 5;
    loadingFlag = true;

    constructor(private messageService: MessageService, private confirmService: ConfirmService) { }

    ngOnInit(): void {
        this.loadMessages();
    }

    loadMessages() {
        this.loadingFlag = true;
        this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(resp => {
            this.messages = resp.result!;
            this.pagination = resp.pagination!;
            this.loadingFlag = false;
        })
    }

    pageChanged(event: any) {
        if (this.pageNumber !== event.page) {
            this.pageNumber = event.page;
            this.loadMessages();
        }
    }

    getLink(message: MessageModel) {
        var output = this.container === "Outbox" ? "/members/" + message.recipientUsername : "/members/" + message.senderUsername;
        return output;
    }

    deleteMessage(id: number) {
        this.confirmService.confirm('Confirm Delete message', 'This cannot be undone...').subscribe(result => {
            if (result) {
                this.messageService.deleteMessage(id).subscribe(() => {
                    this.messages.splice(this.messages.findIndex(x => x.id === id));
                });
            }
        })
    }
}
