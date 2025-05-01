import { Component, Input } from '@angular/core';
import { MemberModel } from '../../../Models/member';
import { RouterLink } from '@angular/router';
import { MembersService } from '../../../services/members.service';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from '../../../services/presence.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent {

    @Input() member?: MemberModel;

    constructor(private memberservice: MembersService, private toastr: ToastrService, public presenceService: PresenceService){ }

    addLike(member: MemberModel){
        this.memberservice.addLike(member.username).subscribe(() => this.toastr.success(`You have liked ${member.knownAs}`));
    }


}
