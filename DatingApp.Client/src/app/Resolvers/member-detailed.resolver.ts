import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { MemberModel } from '../Models/member';
import { Injectable } from '@angular/core';
import { MembersService } from '../services/members.service';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class MembermemberDetailedResolver implements Resolve<MemberModel>{

    constructor(private memberService: MembersService){ }

    resolve(route: ActivatedRouteSnapshot): Observable<MemberModel> {
        return this.memberService.getMember(route.paramMap.get('username')!);
    }
}