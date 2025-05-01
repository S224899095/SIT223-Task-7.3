import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UserService } from './services/user.service';
import { UserModel } from './Models/userModel';
import { AccountService } from './services/account.service';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NavComponent } from './Components/nav/nav.component';
import { PresenceService } from './services/presence.service';
import { take } from 'rxjs';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterOutlet, NavComponent, NgxSpinnerModule],
    templateUrl: './app.component.html',
    styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {

    public title = 'Dating App';
    public users?: UserModel[];

    constructor(private accountService: AccountService, private presenceService: PresenceService) { }

    ngOnInit(): void {
        this.startHubConnection();
    }

    startHubConnection() {
        this.accountService.getCurrentUser().pipe(take(1)).subscribe((user) => {
            if (user !== null) {
                this.presenceService.createHubConnection(user);
            }
        })
    }
}
