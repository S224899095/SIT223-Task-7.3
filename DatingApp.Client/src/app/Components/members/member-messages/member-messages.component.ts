import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { MessageModel } from '../../../Models/message';
import { MessageService } from '../../../services/message.service';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
    changeDetection: ChangeDetectionStrategy.OnPush,
    selector: 'app-member-messages',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './member-messages.component.html',
    styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent implements OnInit {

    @ViewChild('messageForm') messageForm!: NgForm;
    @Input() messages: MessageModel[] = [];
    @Input() username: string = '';
    messageContent: string = '';

    constructor(public messageService: MessageService, private cdRef: ChangeDetectorRef) { }

    ngOnInit(): void {

    }

    SendMessage() {
        this.messageService.sendMessage(this.username, this.messageContent).then(() => {
            this.cdRef.detectChanges();
            this.messageForm.reset();
        });
    }
}
