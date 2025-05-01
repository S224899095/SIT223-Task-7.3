import { Component } from '@angular/core';
import { RegisterComponent } from '../../Components/register/register.component';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
    public registerMode: boolean = false;

    constructor(){}

    public registerToggle(){
        this.registerMode = !this.registerMode;
    }

    public handleCancelRegister(event: boolean){
        this.registerMode = event;
    }
}
