import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  standalone: true,
  imports: [],
  templateUrl: './server-error.component.html',
  styleUrl: './server-error.component.css'
})
export class ServerErrorComponent {

    state: any;
    
    constructor(private router: Router){
        const nav = this.router.getCurrentNavigation();
        this.state = nav?.extras?.state;
    }

}
