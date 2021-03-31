// предикат x <= y,  повертає 1, якщо x<=y, повертає 0, якщо x>y 
|q0->Lq1R
|q1->|q1R
#q1->#q1R
Lq1->Lq2L
|q2->Lq3L 
|q3->|q3L
#q3->#q3L
Lq3->Lq0R
#q0->|q4R
|q4->Lq4R
Lq4->Lq*
#q2->Lq5L
|q5->Lq5L
Lq5->Lq*