-- DELETE FROM public.couriers

INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('bf79a004-56d7-4e5f-a21c-0a9e5e08d10d', 'Петя', 1, 1, 3, 2);
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('a9f7e4aa-becc-40ff-b691-f063c5d04015', 'Оля', 1, 3,2, 2);    
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('db18375d-59a7-49d1-bd96-a1738adcee93', 'Ваня', 2, 4,5, 1);
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('e7c84de4-3261-476a-9481-fb6be211de75', 'Маша', 2, 1,8, 1);    
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('407f68be-5adf-4e72-81bc-b1d8e9574cf8', 'Игорь', 3, 7,9, 1);
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('006e6c66-087e-4a27-aa59-3c0a2bc945c5', 'Даша', 3, 5,5, 1);    
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('40d50b82-ce79-4cde-8ce1-21883f466038', 'Сережа', 4, 7,3, 1);    
    
INSERT INTO public.couriers(
    id, name, transport_id, location_x, location_y, status_id)
    VALUES ('18e5ba41-6710-4143-9808-704e88e94bd9', 'Катя', 4, 6,9, 1);        

SELECT id, name, transport_id, location_x, location_y, status_id
    FROM public.couriers;